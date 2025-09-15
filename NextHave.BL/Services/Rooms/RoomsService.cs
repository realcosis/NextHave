using Dolphin.Core.Backgrounds;
using Dolphin.Core.Exceptions;
using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Clients;
using NextHave.BL.Events.Rooms;
using NextHave.BL.Events.Rooms.Engine;
using NextHave.BL.Events.Rooms.Items;
using NextHave.BL.Localizations;
using NextHave.BL.Mappers;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Models.Rooms.Navigators;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Groups;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Tasks.Rooms;
using NextHave.DAL.Enums;
using NextHave.DAL.Mongo;
using NextHave.DAL.Mongo.Entities;
using NextHave.DAL.MySQL;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Rooms
{
    [Service(ServiceLifetime.Singleton)]
    class RoomsService(IServiceScopeFactory serviceScopeFactory, ILogger<IRoomsService> logger, IGroupsService groupsService) : IRoomsService, IStartableService
    {
        IRoomsService Instance => this;

        ConcurrentDictionary<int, IRoomInstance> IRoomsService.ActiveRooms { get; } = [];

        readonly ConcurrentDictionary<string, RoomModel> RoomModels = [];

        async Task<Room?> IRoomsService.CreateRoom(string name, string description, string modelId, RoomModel model, NavigatorCategory category, User user, int maxPlayers, int tradeType)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();
            var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            var roomId = await mysqlDbContext.GetNextSequenceValue("rooms");
            if (!roomId.HasValue)
                return default;

            var roomEntity = new RoomEntity
            {
                EntityId = roomId,
                Name = name,
                Description = description,
                AccessStatus = RoomAccessStatus.Open,
                Model = new()
                {
                    DoorOrientation = model.DoorOrientation,
                    DoorX = model.DoorX,
                    DoorY = model.DoorY,
                    DoorZ = model.DoorZ,
                    Heightmap = model.Heightmap,
                    ModelId = modelId
                },
                Category = new()
                {
                    Caption = category.Name,
                    CategoryId = category.Id
                },
                MaxUsers = maxPlayers,
                Author = new()
                {
                    AuthorId = user.Id,
                    Name = user.Username
                },
                TradeSettings = tradeType,
                Wallpaper = "0.0",
                Floorpaper = "0.0",
                Landscape = "0.0",
                MuteSettings = 1,
                BanSettings = 1,
                KickSettings = 1,
                WallHeight = 2,
                RollerSpeed = 4
            };

            await mongoDbContext.Rooms.AddAsync(roomEntity);
            await mongoDbContext.SaveChangesAsync();

            var room = roomEntity.Map();

            user.Rooms.Add(room);

            await scope.DisposeAsync();

            return room;
        }

        async Task<Room?> IRoomsService.GetRoom(int roomId)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            var dbRoom = await mongoDbContext.Rooms.AsNoTracking().FirstOrDefaultAsync(g => g.EntityId == roomId);

            var group = dbRoom?.Group != default ? await groupsService.GetGroup(dbRoom.Group.GroupId) : default;

            await scope.DisposeAsync();

            return dbRoom?.Map(group);
        }

        async Task<IRoomInstance?> IRoomsService.GetRoomInstance(int roomId)
        {
            if (Instance.ActiveRooms.TryGetValue(roomId, out var roomInstance))
                return roomInstance;

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var room = await Instance.GetRoom(roomId);
            if (room == null)
                return default;

            var data = scope.ServiceProvider.GetRequiredService<RoomFactory>().GetRoomInstance(roomId, room);
            if (data.Reference == null)
                return default;

            roomInstance = data.Reference;
            if (data.FirstLoad)
            {
                await roomInstance.Init();
                await roomInstance.EventsService.DispatchAsync<RequestRoomGameMapEvent>(new()
                {
                    ModelName = roomInstance.Room!.ModelName,
                    RoomId = roomInstance.Room.Id,
                });
                await roomInstance.EventsService.DispatchAsync<LoadRoomItemsEvent>(new()
                {
                    RoomId = roomInstance.Room.Id
                });
                Instance.ActiveRooms.TryAdd(roomId, roomInstance);
            }

            await scope.DisposeAsync();

            return roomInstance;
        }

        async Task IRoomsService.DisposeRoom(int roomId)
        {
            if (Instance.ActiveRooms.TryGetValue(roomId, out var roomInstance))
            {
                await using var scope = serviceScopeFactory.CreateAsyncScope();

                await roomInstance.EventsService.DispatchAsync<DisposeRoomEvent>(new()
                {
                    RoomId = roomId
                });

                await roomInstance.Dispose();

                scope.ServiceProvider.GetRequiredService<RoomFactory>().DestroyRoomInstance(roomId);

                scope.ServiceProvider.GetRequiredService<RoomEventsFactory>().CleanupRoom(roomId);

                Instance.ActiveRooms.TryRemove(roomId, out _);

                await scope.DisposeAsync();
            }
        }

        async Task<RoomModel?> IRoomsService.GetRoomModel(string modelName, int? roomId)
        {
            if (roomId.HasValue && modelName.Equals("custom", StringComparison.InvariantCultureIgnoreCase))
                return await GetCustomModelData(roomId.Value);

            return await GetModelData(modelName);
        }

        async Task IRoomsService.SaveRoom(Room room, Client client, NavigatorCategory category)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            try
            {
                var entity = await mongoDbContext.Rooms.FirstOrDefaultAsync(r => r.EntityId == room.Id) ?? throw new DolphinException(Errors.RoomNotFound);

                entity.Map(room, category);

                mongoDbContext.Rooms.Update(entity);
                await mongoDbContext.SaveChangesAsync();
            }
            catch (DolphinException exception)
            {
                await client.SendSystemNotification("generic", new()
                {
                    ["message"] = string.Join("\n", exception.Messages)
                });
            }

            await scope.DisposeAsync();
        }

        async Task IStartableService.StartAsync()
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            try
            {

                var backgroundsService = scope.ServiceProvider.GetRequiredService<IBackgroundsService>();
                var task = scope.ServiceProvider.GetRequiredKeyedService<RoomTickTask>("RoomTickTask");
                var cancellationSource = new CancellationTokenSource();
                _ = Task.Run(async () =>
                {
                    using var gameTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(25));
                    while (await gameTimer.WaitForNextTickAsync(cancellationSource.Token))
                        backgroundsService.Queue(task);
                });
            }
            catch (Exception ex)
            {
                logger.LogWarning("Exception during starting of RoomsService: {ex}", ex);
            }
            finally
            {
                await scope.DisposeAsync();
            }
        }

        #region private methods

        async Task<RoomModel?> GetModelData(string modelName)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            var roomModelntity = await mysqlDbContext
                                        .RoomModels
                                            .FirstOrDefaultAsync(rmc => rmc.Id == modelName) ?? throw new DolphinException(Errors.RoomModelNotFound);

            await scope.DisposeAsync();

            return new RoomModel(roomModelntity.DoorX!.Value, roomModelntity.DoorY!.Value, roomModelntity.DoorZ!.Value, roomModelntity.DoorDir!.Value, roomModelntity.HeightMap!);
        }

        async Task<RoomModel> GetCustomModelData(int roomId)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            var roomModelCustomEntity = await mysqlDbContext
                                                .RoomModelCustoms
                                                    .FirstOrDefaultAsync(rmc => rmc.RoomId == roomId) ?? throw new DolphinException(Errors.RoomModelNotFound);

            await scope.DisposeAsync();

            return new RoomModel(roomModelCustomEntity.DoorX!.Value, roomModelCustomEntity.DoorY!.Value, 0.0, roomModelCustomEntity.DoorDir!.Value, roomModelCustomEntity.ModelData!);
        }

        #endregion
    }
}