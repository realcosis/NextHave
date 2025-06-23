using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Mappers;
using NextHave.BL.Models;
using NextHave.BL.Models.Groups;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Models.Rooms.Navigators;
using NextHave.DAL.Mongo;
using NextHave.DAL.MySQL;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Rooms
{
    [Service(ServiceLifetime.Singleton)]
    class RoomsService(IServiceScopeFactory serviceScopeFactory, ILogger<IRoomsService> logger) : IRoomsService, IStartableService
    {
        IRoomsService Instance => this;

        ConcurrentDictionary<int, NavigatorCategory> IRoomsService.NavigatorCategories { get; } = [];

        List<Room> IRoomsService.ActiveRooms { get; } = [];

        async Task<Room?> IRoomsService.GetRoom(int roomId)
        {
            using var scope = serviceScopeFactory.CreateAsyncScope();
            var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
            var dbRoom = await mongoDbContext.Rooms.AsNoTracking().FirstOrDefaultAsync(g => g.EntityId == roomId);
            if (dbRoom != default)
            {
                var group = default(Group?);
                //if (dbRoom.Group != default)
                //    group = await groupsManager.GetGroup(dbRoom.Group.GroupId);

                return dbRoom.Map(group);
            }

            return default;
        }

        async Task<TryGetReference<Room>> IRoomsService.TryGetRoom(int roomId)
        {
            var room = Instance.ActiveRooms.FirstOrDefault(ar => ar.Id == roomId);

            if (room == default)
            {
                room = await Instance.GetRoom(roomId);
                if (room != default)
                    Instance.ActiveRooms.Add(room);
            }

            return new TryGetReference<Room>
            {
                Exists = room != default,
                Reference = room
            };
        }

        async Task IStartableService.StartAsync()
        {
            using var scope = serviceScopeFactory.CreateAsyncScope();
            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();
            Instance.NavigatorCategories.Clear();

            try
            {
                var categories = await mysqlDbContext.NavigatorUserCategories.AsNoTracking().ToListAsync();
                categories.ForEach(category => Instance.NavigatorCategories.TryAdd(category.Id, category.Map()));

                logger.LogInformation("RoomsManager has been loaded with {count} navigator categories definitions", Instance.NavigatorCategories.Count);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Exception during starting of RoomsManager: {ex}", ex);
            }
        }
    }
}