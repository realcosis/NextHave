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
using NextHave.BL.Models.Groups;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Models.Rooms.Navigators;
using NextHave.BL.Services.Groups;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.DAL.Mongo;
using NextHave.DAL.MySQL;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Rooms
{
    [Service(ServiceLifetime.Singleton)]
    class RoomsService(IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILogger<IRoomsService> logger, IGroupsService groupsService) : IRoomsService, IStartableService
    {
        IRoomsService Instance => this;

        ConcurrentDictionary<int, IRoomInstance> IRoomsService.ActiveRooms { get; } = [];

        readonly ConcurrentDictionary<string, RoomModel> RoomModels = [];

        async Task<Room?> IRoomsService.GetRoom(int roomId)
        {
            var mongoDbContext = serviceProvider.GetRequiredService<MongoDbContext>();
            var dbRoom = await mongoDbContext.Rooms.AsNoTracking().FirstOrDefaultAsync(g => g.EntityId == roomId);
            if (dbRoom != default)
            {
                var group = default(Group?);
                if (dbRoom.Group != default)
                    group = await groupsService.GetGroup(dbRoom.Group.GroupId);

                return dbRoom.Map(group);
            }

            return default;
        }

        async Task<IRoomInstance?> IRoomsService.GetRoomInstance(int roomId)
        {
            if (Instance.ActiveRooms.TryGetValue(roomId, out var roomInstance))
                return roomInstance;

            var room = await Instance.GetRoom(roomId);
            if (room != default)
            {
                (roomInstance, var firstLoad) = (await serviceScopeFactory.GetRequiredService<RoomFactory>()).GetRoomInstance(roomId, room);
                if (firstLoad)
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
                }
                Instance.ActiveRooms.TryAdd(roomId, roomInstance);
                return roomInstance;
            }

            return default;
        }

        async Task IRoomsService.DisposeRoom(int roomId)
        {
            if (Instance.ActiveRooms.TryGetValue(roomId, out var roomInstance))
            {
                await roomInstance.EventsService.DispatchAsync<DisposeRoomEvent>(new()
                {
                    RoomId = roomId
                });

                await roomInstance.Dispose();

                (await serviceScopeFactory.GetRequiredService<RoomFactory>()).DestroyRoomInstance(roomId);

                Instance.ActiveRooms.TryRemove(roomId, out _);
            }
        }

        async Task<RoomModel?> IRoomsService.GetRoomModel(string modelName, int roomId)
        {
            if (modelName.Equals("custom", StringComparison.InvariantCultureIgnoreCase))
                return await GetCustomModelData(roomId);

            return await GetModelData(modelName);
        }

        async Task IRoomsService.SaveRoom(Room room, Client client, NavigatorCategory category)
        {
            var mongoDbContext = await serviceScopeFactory.GetRequiredService<MongoDbContext>();
            
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
        }

        async Task IStartableService.StartAsync()
        {
            try
            {
                var cancellationSource = new CancellationTokenSource();
                _ = Task.Run(async () =>
                {
                    using var gameTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(25));
                    while (await gameTimer.WaitForNextTickAsync(cancellationSource.Token))
                    {
                        await Parallel.ForEachAsync(Instance.ActiveRooms.Values, new ParallelOptions()
                        {
                            CancellationToken = cancellationSource.Token,
                            MaxDegreeOfParallelism = Environment.ProcessorCount
                        }, async (roomInstance, token) =>
                        {
                            using var roomTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
                            while (await roomTimer.WaitForNextTickAsync(token))
                            {
                                try
                                {
                                    await roomInstance.OnRoomTick();
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError("Exception with OnRoomTick for room {RoomId}: {ex}", roomInstance.Room!.Id, ex);
                                }
                            }
                        });
                    }
                });
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogWarning("Exception during starting of RoomsService: {ex}", ex);
            }
        }

        #region private methods

        async Task<RoomModel?> GetModelData(string modelName)
        {
            var mysqlDbContext = serviceProvider.GetRequiredService<MySQLDbContext>();

            var roomModelntity = await mysqlDbContext
                                        .RoomModels
                                            .FirstOrDefaultAsync(rmc => rmc.Id == modelName) ?? throw new DolphinException(Errors.RoomModelNotFound);

            return new RoomModel(roomModelntity.DoorX!.Value, roomModelntity.DoorY!.Value, roomModelntity.DoorZ!.Value, roomModelntity.DoorDir!.Value, roomModelntity.HeightMap!);
        }

        async Task<RoomModel> GetCustomModelData(int roomId)
        {
            var mysqlDbContext = serviceProvider.GetRequiredService<MySQLDbContext>();

            var roomModelCustomEntity = await mysqlDbContext
                                                .RoomModelCustoms
                                                    .FirstOrDefaultAsync(rmc => rmc.RoomId == roomId) ?? throw new DolphinException(Errors.RoomModelNotFound);

            return new RoomModel(roomModelCustomEntity.DoorX!.Value, roomModelCustomEntity.DoorY!.Value, 0.0, roomModelCustomEntity.DoorDir!.Value, roomModelCustomEntity.ModelData!);
        }

        #endregion
    }
}