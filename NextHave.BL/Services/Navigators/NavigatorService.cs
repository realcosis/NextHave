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
    class NavigatorService(MySQLDbContext dbContext, ILogger<INavigatorsService> logger,
                           IServiceProvider serviceProvider, IRoomsService roomsService) : INavigatorsService, IStartableService
    {
        ConcurrentDictionary<string, IFilter?> INavigatorsService.Filters { get; } = [];

        ConcurrentDictionary<int, NavigatorPublicCategory> INavigatorsService.PublicCategories { get; } = [];

        async Task IStartableService.StartAsync()
        {
            ((INavigatorsService)this).PublicCategories.Clear();

            try
            {
                var categories = await dbContext.NavigatorPublicCategories.AsNoTracking().ToListAsync();
                categories.ForEach(category => ((INavigatorsService)this).PublicCategories.TryAdd(category.Id, category.Map()));

                var publicRooms = await dbContext.NavigatorPublicRooms.AsNoTracking().ToListAsync();
                foreach (var publicRoom in publicRooms)
                {
                    var category = ((INavigatorsService)this).PublicCategories.FirstOrDefault(pc => pc.Key == publicRoom.CategoryId).Value;
                    if (category != default)
                    {
                        var room = await roomsService.GetRoom(publicRoom.RoomId);
                        if (room != default)
                            category.AddRoom(room);
                    }
                }

                var filters = serviceProvider.GetRequiredService<IEnumerable<IFilter>>();
                foreach (var filter in filters)
                    ((INavigatorsService)this).Filters.TryAdd(filter.Name, filters.FirstOrDefault(f => f.Name == filter.Name));
                
                logger.LogInformation("NavigatorManager has been loaded with {count} public categories definitions", ((INavigatorsService)this).PublicCategories.Count);
                logger.LogInformation("NavigatorManager has been loaded with {count} filters definitions", ((INavigatorsService)this).Filters.Count);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Exception during starting of NavigatorManager: {ex}", ex);
            }
        }
    }
}