using Dolphin.Core.Injection;
using Dolphin.HabboHotel.Navigators.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Mappers;
using NextHave.BL.Models.Navigators;
using NextHave.BL.Services.Rooms;
using NextHave.DAL.MySQL;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Navigators
{
    [Service(ServiceLifetime.Singleton)]
    class NavigatorService(ILogger<INavigatorsService> logger, IServiceScopeFactory serviceScopeFactory, IRoomsService roomsService) : INavigatorsService, IStartableService
    {
        INavigatorsService Instance => this;

        ConcurrentDictionary<string, IFilter?> INavigatorsService.Filters { get; } = [];

        ConcurrentDictionary<int, NavigatorPublicCategory> INavigatorsService.PublicCategories { get; } = [];

        async Task IStartableService.StartAsync()
        {

            using var scope = serviceScopeFactory.CreateAsyncScope();
            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();
            Instance.PublicCategories.Clear();

            try
            {
                var categories = await mysqlDbContext.NavigatorPublicCategories.AsNoTracking().ToListAsync();
                categories.ForEach(category => Instance.PublicCategories.TryAdd(category.Id, category.Map()));

                var publicRooms = await mysqlDbContext.NavigatorPublicRooms.AsNoTracking().ToListAsync();
                foreach (var publicRoom in publicRooms)
                {
                    var category = Instance.PublicCategories.FirstOrDefault(pc => pc.Key == publicRoom.CategoryId).Value;
                    if (category != default)
                    {
                        var room = await roomsService.GetRoom(publicRoom.RoomId);
                        if (room != default)
                            category.AddRoom(room);
                    }
                }

                var filters = scope.ServiceProvider.GetRequiredService<IEnumerable<IFilter>>();
                foreach (var filter in filters)
                    Instance.Filters.TryAdd(filter.Name, filters.FirstOrDefault(f => f.Name == filter.Name));
                
                logger.LogInformation("NavigatorManager has been loaded with {count} public categories definitions", Instance.PublicCategories.Count);
                logger.LogInformation("NavigatorManager has been loaded with {count} filters definitions", Instance.Filters.Count);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Exception during starting of NavigatorManager: {ex}", ex);
            }
        }
    }
}