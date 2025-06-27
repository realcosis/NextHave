using Dolphin.Core.Events;
using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Clients;
using NextHave.BL.Events.Rooms;
using NextHave.BL.Events.Rooms.Movements;
using NextHave.BL.Events.Users;
using NextHave.BL.Messages.Input.Handshake;
using NextHave.BL.Messages.Input.Rooms;
using NextHave.BL.Messages.Output;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Packets;
using NextHave.BL.Services.Rooms;
using NextHave.BL.Services.Users;
using NextHave.BL.Utils;

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

            await Task.CompletedTask;
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
                await using var floorHeightMapMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.FloorHeightMapMessageComposer);
                roomInstance.RoomModel!.SerializeHeightmap(floorHeightMapMessageComposer);
                await client.Send(floorHeightMapMessageComposer.Bytes());

                await using var heightMapMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.HeightMapMessageComposer);
                roomInstance.RoomModel!.SerializeHeight(heightMapMessageComposer);
                await client.Send(heightMapMessageComposer.Bytes());

                await using var roomEntryInfoMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.RoomEntryInfoMessageComposer);
                roomEntryInfoMessageComposer.AddInt32(roomInstance.Room!.Id);
                roomEntryInfoMessageComposer.AddBoolean(true);
                await client.Send(roomEntryInfoMessageComposer.Bytes());

                await roomInstance.EventsService.DispatchAsync<AddUserToRoomEvent>(new()
                {
                    RoomId = client.User.CurrentRoomId!.Value,
                    User = client.User,
                    Spectator = false
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
            {

                await using var roomReadyMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.RoomReadyMessageComposer);
                roomReadyMessageComposer.AddString(roomInstance.Room.ModelName!);
                roomReadyMessageComposer.AddInt32(roomInstance.Room.Id);
                await client.Send(roomReadyMessageComposer.Bytes());

            }
        }

        public async Task OnOpenFlatMessage(OpenFlatMessage message, Client client)
        {
            if (client?.User == default)
                return;

            using var scope = serviceScopeFactory.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;
            var roomsService = serviceProvider.GetRequiredService<IRoomsService>();

            var roomInstance = await roomsService.GetRoomInstance(message.RoomId);
            if (roomInstance?.Room != default)
            {
                await using var rouArePlayingGameMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.YouArePlayingGameMessageComposer);
                rouArePlayingGameMessageComposer.AddBoolean(false);
                await client.Send(rouArePlayingGameMessageComposer.Bytes());

                await using var prepareRoomMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.PrepareRoomMessageComposer);
                prepareRoomMessageComposer.AddInt32(roomInstance.Room.Id);
                await client.Send(prepareRoomMessageComposer.Bytes());

                await using var roomReadyMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.RoomReadyMessageComposer);
                roomReadyMessageComposer.AddString(roomInstance.Room.ModelName!);
                roomReadyMessageComposer.AddInt32(roomInstance.Room.Id);
                await client.Send(roomReadyMessageComposer.Bytes());

                await roomInstance.EventsService.DispatchAsync(new RequestRoomGameMapEvent
                {
                    ModelName = roomInstance.Room.ModelName,
                    RoomId = roomInstance.Room.Id,
                });

                client.User.CurrentRoomId = roomInstance.Room.Id;
            }
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
            await using var authenticationOKComposer = ServerMessageFactory.GetServerMessage(OutputCode.AuthenticationOKMessageComposer);
            await client.Send(authenticationOKComposer.Bytes());

            await using var availabilityStatusMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.AvailabilityStatusMessageComposer);
            availabilityStatusMessageComposer.AddBoolean(true);
            availabilityStatusMessageComposer.AddBoolean(false);
            availabilityStatusMessageComposer.AddBoolean(true);
            await client.Send(availabilityStatusMessageComposer.Bytes());

            await using var userRightsMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.UserRightsMessageComposer);
            userRightsMessageComposer.AddInt32(2);
            userRightsMessageComposer.AddInt32(user.Rank);
            userRightsMessageComposer.AddBoolean(false);
            await client.Send(userRightsMessageComposer.Bytes());

            await using var navigatorHomeRoomMessageEvent = ServerMessageFactory.GetServerMessage(OutputCode.NavigatorHomeRoomMessageComposer);
            navigatorHomeRoomMessageEvent.AddInt32(user.HomeRoom ?? 0);
            navigatorHomeRoomMessageEvent.AddInt32(user.HomeRoom ?? 0);
            await client.Send(navigatorHomeRoomMessageEvent.Bytes());
        }

        static async Task SendInfoRetrieveResponse(Client client, User user)
        {
            await using var userObjectMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.UserObjectMessageComposer);

            userObjectMessageComposer.AddInt32(user.Id);
            userObjectMessageComposer.AddString(user.Username!);
            userObjectMessageComposer.AddString(user.Look!);
            userObjectMessageComposer.AddString(user.Gender!.ToUpper());
            userObjectMessageComposer.AddString(user.Motto ?? string.Empty);
            userObjectMessageComposer.AddString(string.Empty);
            userObjectMessageComposer.AddBoolean(false);
            userObjectMessageComposer.AddInt32(0);
            userObjectMessageComposer.AddInt32(0);
            userObjectMessageComposer.AddInt32(0);
            userObjectMessageComposer.AddBoolean(false);
            userObjectMessageComposer.AddString(user.LastOnline!.Value.GetDifference().ToString());
            userObjectMessageComposer.AddBoolean(false);
            userObjectMessageComposer.AddBoolean(false);

            await client.Send(userObjectMessageComposer.Bytes());
        }

        #endregion
    }
}