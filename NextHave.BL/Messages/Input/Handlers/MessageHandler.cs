using Dolphin.Core.Backgrounds;
using Dolphin.Core.Extensions;
using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Clients;
using NextHave.BL.Events.Rooms.Chat;
using NextHave.BL.Events.Rooms.Engine;
using NextHave.BL.Events.Rooms.Items;
using NextHave.BL.Events.Rooms.Session;
using NextHave.BL.Events.Rooms.Users;
using NextHave.BL.Events.Rooms.Users.Movements;
using NextHave.BL.Events.Users.Friends;
using NextHave.BL.Events.Users.Messenger;
using NextHave.BL.Events.Users.Session;
using NextHave.BL.Messages.Input.Friends;
using NextHave.BL.Messages.Input.Handshake;
using NextHave.BL.Messages.Input.Messenger;
using NextHave.BL.Messages.Input.Navigators;
using NextHave.BL.Messages.Input.Rooms;
using NextHave.BL.Messages.Input.Rooms.Chat;
using NextHave.BL.Messages.Input.Rooms.Connection;
using NextHave.BL.Messages.Input.Rooms.Engine;
using NextHave.BL.Messages.Input.Rooms.Settings;
using NextHave.BL.Messages.Output.Handshake;
using NextHave.BL.Messages.Output.Navigators;
using NextHave.BL.Messages.Output.roomInstances.Settings;
using NextHave.BL.Messages.Output.Rooms.Engine;
using NextHave.BL.Messages.Output.Rooms.Session;
using NextHave.BL.Messages.Output.Rooms.Settings;
using NextHave.BL.Messages.Output.Users;
using NextHave.BL.Messages.Parsers.Navigators;
using NextHave.BL.Services.Navigators;
using NextHave.BL.Services.Packets;
using NextHave.BL.Services.Rooms;
using NextHave.BL.Services.Texts;
using NextHave.BL.Services.Users;
using NextHave.BL.Tasks.Rooms.Settings;
using NextHave.DAL.Enums;

namespace NextHave.BL.Messages.Input.Handlers
{
    [Service(ServiceLifetime.Singleton)]
    class MessageHandler(IPacketsService packetsService, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) : IMessageHandler, IStartableService
    {
        async Task IStartableService.StartAsync()
        {
            packetsService.Subscribe<SSOTicketMessage>(this, OnSSOTicket);

            packetsService.Subscribe<InfoRetrieveMessage>(this, OnInfoRetrieve);

            packetsService.Subscribe<OpenFlatMessage>(this, OnOpenFlatMessage);

            packetsService.Subscribe<GetRoomEntryDataMessage>(this, OnGetRoomEntryDataMessage);

            packetsService.Subscribe<GetFurnitureAliasesMessage>(this, OnGetFurnitureAliasesMessage);

            packetsService.Subscribe<MoveAvatarMessage>(this, OnMoveAvatar);

            packetsService.Subscribe<MoveObjectMessage>(this, OnMoveObject);

            packetsService.Subscribe<ChatMessageMessage>(this, OnChatMessage);

            packetsService.Subscribe<ShoutMessageMessage>(this, OnShoutMessage);

            packetsService.Subscribe<GetGuestRoomMessage>(this, OnGetGuestRoom);

            packetsService.Subscribe<StartTypingMessage>(this, OnStartTyping);

            packetsService.Subscribe<StopTypingMessage>(this, OnStopTyping);

            packetsService.Subscribe<NewNavigatorSearchMessage>(this, OnNewNavigatorSearchMessage);

            packetsService.Subscribe<NewNavigatorInitMessage>(this, OnNewNavigatorInitMessage);

            packetsService.Subscribe<GetUserFlatCatsMessage>(this, OnGetUserFlatCatsMessage);

            packetsService.Subscribe<GoToHotelViewMessage>(this, OnGoToHotelViewMessage);

            packetsService.Subscribe<MessengerInitMessage>(this, OnMessengerInitMessage);

            packetsService.Subscribe<SendMessageMessage>(this, OnSendMessageMessage);

            packetsService.Subscribe<SaveRoomSettingsMessage>(this, OnSaveRoomSettingsMessage);

            packetsService.Subscribe<GetRoomSettingsMessage>(this, OnGetRoomSettingsMessage);

            packetsService.Subscribe<CreateFlatMessage>(this, OnCreateFlatMessage);

            await Task.CompletedTask;
        }

        async Task OnCreateFlatMessage(CreateFlatMessage message, Client client)
        {
            if (client?.UserInstance?.User == default)
                return;

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var textsService = scope.ServiceProvider.GetRequiredService<ITextsService>();

            var roomsService = scope.ServiceProvider.GetRequiredService<IRoomsService>();

            var navigatorsService = scope.ServiceProvider.GetRequiredService<INavigatorsService>();

            if (string.IsNullOrWhiteSpace(message.ModelName))
                return;

            if (string.IsNullOrWhiteSpace(message.Name) || (!string.IsNullOrWhiteSpace(message.Name) && message.Name.Length < 3) || (!string.IsNullOrWhiteSpace(message.Name) && message.Name.Length > 25))
            {
                await client.SendSystemNotification("generic", new()
                {
                    ["message"] = textsService.GetText("nexthave_room_name_not_valid", "Room name not valid.")
                });
                return;
            }

            var model = await roomsService.GetRoomModel(message.ModelName!);
            if (model == default)
            {
                await client.SendSystemNotification("generic", new()
                {
                    ["message"] = textsService.GetText("nexthave_room_model_not_found", "Room model not found.")
                });
                return;
            }

            if (!navigatorsService.NavigatorCategories.TryGetValue(message.CategoryId, out var navigatorCategory))
            {
                await client.SendSystemNotification("generic", new()
                {
                    ["message"] = textsService.GetText("nexthave_room_category_not_found", "Room category not found.")
                });
                return;
            }

            if (navigatorCategory.MinRank > client.UserInstance.User.Rank)
            {
                await client.SendSystemNotification("generic", new()
                {
                    ["message"] = textsService.FormatText("nexthave_room_category_not_valid", "Room category not valid.", navigatorCategory.Name!)
                });
                return;
            }

            var tradeType = message.TradeType;
            if (tradeType < 0 || tradeType > 2)
                tradeType = 0;

            var room = await roomsService.CreateRoom(message.Name, message.Description ?? string.Empty, message.ModelName, model!, navigatorCategory, client.UserInstance.User, message.MaxPlayers, tradeType);
            if (room == default)
                return;

            await client.Send(new FlatCreatedMessageComposer(room.Id, message.Name));

            await scope.DisposeAsync();
        }

        async Task OnSaveRoomSettingsMessage(SaveRoomSettingsMessage message, Client client)
        {
            if (client?.UserInstance?.User == default)
                return;

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var roomsService = scope.ServiceProvider.GetRequiredService<IRoomsService>();

            var navigatorsService = scope.ServiceProvider.GetRequiredService<INavigatorsService>();

            var textsService = scope.ServiceProvider.GetRequiredService<ITextsService>();

            var backgroundsService = scope.ServiceProvider.GetRequiredService<IBackgroundsService>();

            var task = scope.ServiceProvider.GetRequiredKeyedService<SaveRoomSettingsTask>("SaveRoomSettingsTask");

            var roomInstance = await roomsService.GetRoomInstance(message.RoomId);
            if (roomInstance?.Room == default)
                return;

            if (!navigatorsService.NavigatorCategories.TryGetValue(message.CategoryId, out var category))
            {
                await client.SendSystemNotification("generic", new()
                {
                    ["message"] = textsService.GetText("nexthave_room_category_not_found", "Room category not found.")
                });
                return;
            }

            if (category.MinRank > client.UserInstance.User.Rank || (!roomInstance.CheckRights(client.UserInstance, true) && client.UserInstance.User.Rank >= category.MinRank))
            {
                await client.SendSystemNotification("generic", new()
                {
                    ["message"] = textsService.FormatText("nexthave_room_category_not_valid", "Room category not valid.", category.Name!)
                });
                return;
            }

            if (!roomInstance.CheckRights(client.UserInstance!, true))
                return;

            roomInstance.Room.UpdateCache(message);

            if (task != default)
            {
                task.Parameters.TryAdd("room", roomInstance.Room.JsonSerialize(false)!);
                task.Parameters.TryAdd("client", client.ConnectionId!);
                task.Parameters.TryAdd("category", category.JsonSerialize(false)!);
                backgroundsService.Queue(task);
            }

            await client.Send(new RoomSettingsSavedMessageComposer(message.RoomId));
            await client.Send(new RoomInfoUpdatedMessageComposer(message.RoomId));
            await client.Send(new RoomVisualizationSettingsMessageComposer(roomInstance.Room.AllowHidewall, roomInstance.Room.WallThickness, roomInstance.Room.FloorThickness));

            await scope.DisposeAsync();
        }

        async Task OnGetRoomSettingsMessage(GetRoomSettingsMessage message, Client client)
        {
            var roomsService = serviceProvider.GetRequiredService<IRoomsService>();

            var roomInstance = await roomsService.GetRoomInstance(message.RoomId);
            if (roomInstance == default)
                return;

            if (!roomInstance.CheckRights(client.UserInstance!, true))
                return;

            await client.Send(new RoomSettingsDataMessageComposer(roomInstance));
        }

        async Task OnSendMessageMessage(SendMessageMessage message, Client client)
        {
            if (client.UserInstance?.User == default)
                return;

            await client.UserInstance.EventsService.DispatchAsync<SendMessageEvent>(new()
            {
                Message = message.Message,
                UserToId = message.UserId,
                UserId = client.UserInstance.User.Id
            });
        }

        async Task OnMessengerInitMessage(MessengerInitMessage message, Client client)
        {
            if (client.UserInstance == default)
                return;

            await client.UserInstance.EventsService.DispatchAsync<MessengerInitMessageEvent>(new()
            {
                UserId = client.UserInstance!.User!.Id
            });
        }

        async Task OnGoToHotelViewMessage(GoToHotelViewMessage message, Client client)
        {
            if (client?.UserInstance?.CurrentRoomInstance == default || !client.UserInstance.CurrentRoomId.HasValue)
                return;

            var roomsService = serviceProvider.GetRequiredService<IRoomsService>();

            var roomInstance = await roomsService.GetRoomInstance(client.UserInstance.CurrentRoomId.Value);
            if (roomInstance == default)
                return;

            await roomInstance.EventsService.DispatchAsync<UserRoomExitEvent>(new()
            {
                UserId = client.UserInstance!.User!.Id,
                NotifyUser = true,
                Kick = false,
                RoomId = client.UserInstance.CurrentRoomId!.Value,
            });
            client.UserInstance.CurrentRoomId = default;
        }

        async Task OnGetUserFlatCatsMessage(GetUserFlatCatsMessage message, Client client)
        {
            if (client?.UserInstance?.User == default)
                return;

            var navigatorsService = serviceProvider.GetRequiredService<INavigatorsService>();

            var categories = navigatorsService.NavigatorCategories.Select(nc => nc.Value).Where(nc => nc.MinRank <= client.UserInstance.User.Rank).ToList();

            await client.Send(new UserFlatCatsMessageComposer(categories));
        }

        async Task OnNewNavigatorInitMessage(NewNavigatorInitMessage message, Client client)
        {
            if (client?.UserInstance == default)
                return;

            await client.Send(new NavigatorMetaDataMessageComposer());
            await client.Send(new CollapsedCategoriesMessageComposer());
        }

        async Task OnNewNavigatorSearchMessage(NewNavigatorSearchMessage message, Client client)
        {
            if (client?.UserInstance == default)
                return;
            var navigatorsService = serviceProvider.GetRequiredService<INavigatorsService>();

            var filter = navigatorsService.Filters.FirstOrDefault(f => f.Key == message.View).Value;
            if (filter == default)
                return;

            var result = await filter.GetSearchResults(client.UserInstance, message.Query);
            await client.Send(new NavigatorSearchResultBlocksMessageComposer(message.View!, message.Query!, result));
        }

        async Task OnStopTyping(StopTypingMessage message, Client client)
        {
            if (client?.UserInstance?.User == default || !client.UserInstance.CurrentRoomId.HasValue || client.UserInstance.CurrentRoomInstance == default)
                return;

            await client.UserInstance.CurrentRoomInstance.EventsService.DispatchAsync<GetUserVirtualIdEvent>(new()
            {
                UserId = client.UserInstance.User.Id,
                Type = nameof(StopTypingMessage),
                RoomId = client.UserInstance.CurrentRoomId.Value
            });
        }

        async Task OnStartTyping(StartTypingMessage message, Client client)
        {
            if (client?.UserInstance?.User == default || !client.UserInstance.CurrentRoomId.HasValue || client.UserInstance.CurrentRoomInstance == default)
                return;

            await client.UserInstance.CurrentRoomInstance.EventsService.DispatchAsync<GetUserVirtualIdEvent>(new()
            {
                UserId = client.UserInstance.User.Id,
                Type = nameof(StartTypingMessage),
                RoomId = client.UserInstance.CurrentRoomId.Value
            });
        }

        async Task OnGetGuestRoom(GetGuestRoomMessage message, Client client)
        {
            var roomsService = serviceProvider.GetRequiredService<IRoomsService>();

            var roomInstance = await roomsService.GetRoomInstance(message.RoomId);

            if (roomInstance?.Room == default)
                return;

            await client.Send(new GetGuestRoomResultMessageComposer(roomInstance.Room, roomInstance.CheckRights(client.UserInstance!, true), message.IsLoading, message.CheckEntry));
        }

        async Task OnChatMessage(ChatMessageMessage message, Client client)
        {
            if (client?.UserInstance == default || !client.UserInstance.CurrentRoomId.HasValue || client.UserInstance.CurrentRoomInstance == default)
                return;

            await client.UserInstance.CurrentRoomInstance.EventsService.DispatchAsync<GetVirtualIdForChatEvent>(new()
            {
                Color = message.Color,
                Message = message.Message,
                UserId = client.UserInstance.User!.Id,
                Shout = false,
                RoomId = client.UserInstance.CurrentRoomId!.Value
            });
        }

        async Task OnShoutMessage(ShoutMessageMessage message, Client client)
        {
            if (client?.UserInstance == default || !client.UserInstance.CurrentRoomId.HasValue || client.UserInstance.CurrentRoomInstance == default)
                return;

            await client.UserInstance.CurrentRoomInstance.EventsService.DispatchAsync<GetVirtualIdForChatEvent>(new()
            {
                Color = message.Color,
                Message = message.Message,
                UserId = client.UserInstance.User!.Id,
                Shout = true,
                RoomId = client.UserInstance.CurrentRoomId!.Value
            });
        }

        async Task OnMoveObject(MoveObjectMessage message, Client client)
        {
            if (client?.UserInstance == default || !client.UserInstance.CurrentRoomId.HasValue || client.UserInstance.CurrentRoomInstance == default)
                return;

            await client.UserInstance.CurrentRoomInstance.EventsService.DispatchAsync<MoveObjectEvent>(new()
            {
                RoomId = client.UserInstance.CurrentRoomId!.Value,
                ItemId = message.ItemId,
                Rotation = message.Rotation,
                NewY = message.NewY,
                NewX = message.NewX
            });
        }

        async Task OnMoveAvatar(MoveAvatarMessage message, Client client)
        {
            if (client?.UserInstance == default)
                return;

            if (!client.UserInstance.CurrentRoomId.HasValue)
                return;


            if (client.UserInstance.CurrentRoomInstance == default)
                return;

            await client.UserInstance.CurrentRoomInstance.EventsService.DispatchAsync<MoveAvatarEvent>(new()
            {
                RoomId = client.UserInstance.CurrentRoomId!.Value,
                NewX = message.NewX,
                NewY = message.NewY,
                UserId = client.UserInstance!.User!.Id
            });
        }

        async Task OnGetRoomEntryDataMessage(GetRoomEntryDataMessage message, Client client)
        {
            if (client?.UserInstance == default)
                return;

            if (!client.UserInstance.CurrentRoomId.HasValue)
                return;

            var roomsService = serviceProvider.GetRequiredService<IRoomsService>();

            var roomInstance = await roomsService.GetRoomInstance(client.UserInstance.CurrentRoomId.Value);
            if (roomInstance?.Room != default && roomInstance?.RoomModel != default)
            {
                await client.Send(new FloorHeightMapMessageComposer(roomInstance.RoomModel));
                await client.Send(new HeightMapMessageComposer(roomInstance.RoomModel));
                await client.Send(new RoomEntryInfoMessageComposer(roomInstance.Room.Id, false));

                await roomInstance.EventsService.DispatchAsync<AddUserToRoomEvent>(new()
                {
                    RoomId = roomInstance.Room.Id,
                    User = client.UserInstance,
                    Spectator = false
                });

                await roomInstance.EventsService.DispatchAsync<SendItemsToNewUserEvent>(new()
                {
                    Client = client,
                    RoomId = roomInstance.Room.Id,
                });
            }
        }

        async Task OnGetFurnitureAliasesMessage(GetFurnitureAliasesMessage message, Client client)
        {
            if (client?.UserInstance == default)
                return;

            if (!client.UserInstance.CurrentRoomId.HasValue)
                return;

            var roomsService = serviceProvider.GetRequiredService<IRoomsService>();

            var roomInstance = await roomsService.GetRoomInstance(client.UserInstance.CurrentRoomId.Value);
            if (roomInstance?.Room != default)
                await client.Send(new FurnitureAliasesMessageComposer());
        }

        async Task OnOpenFlatMessage(OpenFlatMessage message, Client client)
        {
            if (client?.UserInstance == default)
                return;

            var roomsService = serviceProvider.GetRequiredService<IRoomsService>();

            if (client.UserInstance.CurrentRoomInstance?.Room != default)
                await client.UserInstance.CurrentRoomInstance.EventsService.DispatchAsync<UserRoomExitEvent>(new()
                {
                    UserId = client.UserInstance!.User!.Id,
                    NotifyUser = false,
                    Kick = false,
                    RoomId = client.UserInstance.CurrentRoomInstance.Room.Id,
                });

            var roomInstance = await roomsService.GetRoomInstance(message.RoomId);
            if (roomInstance?.Room == default)
            {
                await client.Send(new CloseConnectionMessageComposer());
                return;
            }

            client.UserInstance.CurrentRoomId = roomInstance.Room.Id;

            await client.Send(new OpenConnectionMessageComposer(roomInstance.Room.Id));

            if (!roomInstance.CheckRights(client.UserInstance, true))
            {
                if (roomInstance.Room.State == RoomAccessStatus.Locked && !client.UserInstance.Permission!.HasRight("nexthave_enter_locked_room"))
                {
                    if (roomInstance.Room.UsersNow > 0)
                    {
                        await client.Send(new DoorbellMessageComposer(string.Empty));
                        await roomInstance.EventsService.DispatchAsync<SendRoomPacketEvent>(new()
                        {
                            Composer = new DoorbellMessageComposer(client.UserInstance!.User!.Username!),
                            WithRights = true,
                            RoomId = roomInstance.Room.Id,
                        });
                        return;
                    }

                    await client.Send(new FlatAccessDeniedMessageComposer(string.Empty));
                    await client.Send(new CloseConnectionMessageComposer());
                    return;
                }

                if (roomInstance.Room.State == RoomAccessStatus.Password && !client.UserInstance.Permission!.HasRight("nexthave_enter_locked_room"))
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
        }

        async Task OnSSOTicket(SSOTicketMessage message, Client client)
        {
            var usersService = serviceProvider.GetRequiredService<IUsersService>();
            var userInstance = await usersService.LoadHabbo(message.SSO!, message.ElapsedMilliseconds);

            if (userInstance == default)
                return;

            userInstance.Client = client;
            client.UserInstance = userInstance;

            await client.Send(new AuthenticationOKMessageComposer());
            await client.Send(new AvailabilityStatusMessageComposer(true, false, true));
            await client.Send(new UserRightsMessageComposer(2, userInstance.Permission!.SecurityLevel!.Value, true));
            await client.Send(new NavigatorHomeRoomMessageComposer(userInstance.User!.HomeRoom ?? 0, userInstance.User!.HomeRoom ?? 0));

            await userInstance.EventsService.DispatchAsync<FriendStatusChangedEvent>(new()
            {
                UserId = userInstance.User!.Id,
                Notification = true
            });

            await userInstance.EventsService.DispatchAsync<UserConnectedEvent>(new()
            {
                UserId = userInstance.User!.Id
            });
        }

        async Task OnInfoRetrieve(InfoRetrieveMessage message, Client client)
        {
            if (client?.UserInstance == default)
                return;

            await client.Send(new UserObjectMessageComposer(client.UserInstance.User!));
        }
    }
}