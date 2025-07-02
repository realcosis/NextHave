using Dolphin.Core.Events;
using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Clients;
using NextHave.BL.Events.Rooms.Engine;
using NextHave.BL.Events.Rooms.Items;
using NextHave.BL.Events.Rooms.Session;
using NextHave.BL.Events.Rooms.Users;
using NextHave.BL.Events.Rooms.Users.Movements;
using NextHave.BL.Events.Users;
using NextHave.BL.Messages.Input.Handshake;
using NextHave.BL.Messages.Input.Rooms;
using NextHave.BL.Messages.Input.Rooms.Connection;
using NextHave.BL.Messages.Input.Rooms.Engine;
using NextHave.BL.Messages.Output;
using NextHave.BL.Messages.Output.Handshake;
using NextHave.BL.Messages.Output.Navigators;
using NextHave.BL.Messages.Output.Rooms.Engine;
using NextHave.BL.Messages.Output.Rooms.Session;
using NextHave.BL.Messages.Output.Users;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Packets;
using NextHave.BL.Services.Rooms;
using NextHave.BL.Services.Users;
using NextHave.BL.Utils;
using NextHave.DAL.Enums;

namespace NextHave.BL.Messages.Input.Handlers
{
    [Service(ServiceLifetime.Singleton)]
    class MessageHandler(IPacketsService packetsService, IServiceScopeFactory serviceScopeFactory) : IMessageHandler, IStartableService
    {
        public async Task StartAsync()
        {

            packetsService.Subscribe<SSOTicketMessage>(this, OnSSOTicket);

            packetsService.Subscribe<InfoRetrieveMessage>(this, OnInfoRetrieve);

            packetsService.Subscribe<OpenFlatMessage>(this, OnOpenFlatMessage);

            packetsService.Subscribe<GetRoomEntryDataMessage>(this, OnGetRoomEntryDataMessage);

            packetsService.Subscribe<GetFurnitureAliasesMessage>(this, OnGetFurnitureAliasesMessage);

            packetsService.Subscribe<MoveAvatarMessage>(this, OnMoveAvatar);

            packetsService.Subscribe<MoveObjectMessage>(this, OnMoveObject);

            await Task.CompletedTask;
        }

        public async Task OnMoveObject(MoveObjectMessage message, Client client)
        {
            if (client?.User == default || !client.User.CurrentRoomId.HasValue || client.User.CurrentRoomInstance == default)
                return;

            await client.User.CurrentRoomInstance.EventsService.DispatchAsync<MoveObjectEvent>(new()
            {
                RoomId = client.User.CurrentRoomId!.Value,
                ItemId = message.ItemId,
                Rotation = message.Rotation,
                NewY = message.NewY,
                NewX = message.NewX
            });
        }

        public async Task OnMoveAvatar(MoveAvatarMessage message, Client client)
        {
            if (client?.User == default)
                return;

            if (!client.User.CurrentRoomId.HasValue)
                return;


            if (client.User.CurrentRoomInstance == default)
                return;

            await client.User.CurrentRoomInstance.EventsService.DispatchAsync<MoveAvatarEvent>(new()
            {
                RoomId = client.User.CurrentRoomId!.Value,
                NewX = message.NewX,
                NewY = message.NewY,
                UserId = client.User.Id
            });
        }

        public async Task OnGetRoomEntryDataMessage(GetRoomEntryDataMessage message, Client client)
        {
            if (client?.User == default)
                return;

            if (!client.User.CurrentRoomId.HasValue)
                return;

            using var scope = serviceScopeFactory.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;
            var roomsService = serviceProvider.GetRequiredService<IRoomsService>();

            var roomInstance = await roomsService.GetRoomInstance(client.User.CurrentRoomId.Value);
            if (roomInstance?.Room != default && roomInstance?.RoomModel != default)
            {
                await client.Send(new FloorHeightMapMessageComposer(roomInstance.RoomModel));
                await client.Send(new HeightMapMessageComposer(roomInstance.RoomModel));
                await client.Send(new RoomEntryInfoMessageComposer(roomInstance.Room.Id, false));

                await roomInstance.EventsService.DispatchAsync<AddUserToRoomEvent>(new()
                {
                    RoomId = roomInstance.Room.Id,
                    User = client.User,
                    Spectator = false
                });

                await roomInstance.EventsService.DispatchAsync<SendItemsToNewUserEvent>(new()
                {
                    Client = client,
                    RoomId = roomInstance.Room.Id,
                });
            }
        }

        public async Task OnGetFurnitureAliasesMessage(GetFurnitureAliasesMessage message, Client client)
        {
            if (client?.User == default)
                return;

            if (!client.User.CurrentRoomId.HasValue)
                return;

            using var scope = serviceScopeFactory.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;
            var roomsService = serviceProvider.GetRequiredService<IRoomsService>();

            var roomInstance = await roomsService.GetRoomInstance(client.User.CurrentRoomId.Value);
            if (roomInstance?.Room != default)
                await client.Send(new FurnitureAliasesMessageComposer());
        }

        public async Task OnOpenFlatMessage(OpenFlatMessage message, Client client)
        {
            if (client?.User == default)
                return;

            using var scope = serviceScopeFactory.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;
            var roomsService = serviceProvider.GetRequiredService<IRoomsService>();
            var eventsService = serviceProvider.GetRequiredService<IEventsService>();

            if (client.User.CurrentRoomInstance != default)
            {
                await client.User.CurrentRoomInstance.EventsService.DispatchAsync(new UserRoomExitEvent
                {
                    UserId = client.User.Id,
                    RoomId = client.User.CurrentRoomId!.Value,
                });
                client.User.CurrentRoomInstance = default;
                client.User.CurrentRoomId = default;
            }

            var roomInstance = await roomsService.GetRoomInstance(message.RoomId);
            if (roomInstance?.Room == default)
            {
                await client.Send(new CloseConnectionMessageComposer());
                return;
            }

            await client.Send(new OpenConnectionMessageComposer(roomInstance.Room.Id));

            if (!roomInstance.CheckRights(client.User, true))
            {
                if (roomInstance.Room.State == RoomAccessStatus.Locked && !client.User.Permission!.HasRight("nexthave_enter_locked_room"))
                {
                    if (roomInstance.Room.UsersNow > 0)
                    {
                        await client.Send(new DoorbellMessageComposer(string.Empty));
                        await roomInstance.EventsService.DispatchAsync(new SendRoomPacketEvent
                        {
                            Composer = new DoorbellMessageComposer(client.User.Username!),
                            WithRights = true,
                            RoomId = roomInstance.Room.Id,
                        });
                        return;
                    }

                    await client.Send(new FlatAccessDeniedMessageComposer(string.Empty));
                    await client.Send(new CloseConnectionMessageComposer());
                    return;
                }

                if (roomInstance.Room.State == RoomAccessStatus.Password && !client.User.Permission!.HasRight("nexthave_enter_locked_room"))
                {
                    if (!message.Password!.ToLower().Equals(roomInstance.Room.Password!.ToLower()) || string.IsNullOrWhiteSpace(message.Password))
                    {
                        await client.Send(new GenericErrorMessageComposer(-100002));
                        await client.Send(new CloseConnectionMessageComposer());
                        return;
                    }
                }
            }

            await client.Send(new RoomReadyMessageComposer(roomInstance.Room.Id, roomInstance.Room.ModelName!));

            if (!string.IsNullOrWhiteSpace(roomInstance.Room.Wallpaper) && !roomInstance.Room.Wallpaper.Equals("0.0"))
                await client.Send(new RoomPropertyMessageComposer("wallpaper", roomInstance.Room.Wallpaper!));
            if (!string.IsNullOrWhiteSpace(roomInstance.Room.Floor) && !roomInstance.Room.Floor.Equals("0.0"))
                await client.Send(new RoomPropertyMessageComposer("floor", roomInstance.Room.Floor!));

            await client.Send(new RoomPropertyMessageComposer("landscape", roomInstance.Room.Landscape!));

            client.User.CurrentRoomInstance = roomInstance;
            client.User.CurrentRoomId = roomInstance.Room.Id;
        }

        public async Task OnSSOTicket(SSOTicketMessage message, Client client)
        {
            using var scope = serviceScopeFactory.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;
            var usersService = serviceProvider.GetRequiredService<IUsersService>();
            var user = await usersService.LoadHabbo(message.SSO!, message.ElapsedMilliseconds);

            if (user == default)
                return;

            var eventsService = serviceProvider.GetRequiredService<IEventsService>();
            user.Client = client;
            client.User = user;
            await SendSSOTicketResponse(client, user);
            await eventsService.DispatchAsync<UserConnectedEvent>(new()
            {
                UserId = user.Id
            });
        }

        public async Task OnInfoRetrieve(InfoRetrieveMessage message, Client client)
        {
            if (client?.User == default)
                return;

            await SendInfoRetrieveResponse(client, client.User!);
        }

        #region private methods

        static async Task SendSSOTicketResponse(Client client, User user)
        {
            await client.Send(new AuthenticationOKMessageComposer());
            await client.Send(new AvailabilityStatusMessageComposer(true, false, true));
            await client.Send(new UserRightsMessageComposer(2, user.Permission!.SecurityLevel!.Value, true));
            await client.Send(new NavigatorHomeRoomMessageComposer(user.HomeRoom ?? 0, user.HomeRoom ?? 0));
        }

        static async Task SendInfoRetrieveResponse(Client client, User user)
        {
            await client.Send(new UserObjectMessageComposer(user));
        }

        #endregion
    }
}