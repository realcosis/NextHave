using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Mappers;
using NextHave.BL.Models.Navigators;
using NextHave.BL.Models.Rooms.Navigators;
using NextHave.BL.Services.Navigators.Filters;
using NextHave.BL.Services.Rooms;
using NextHave.DAL.MySQL;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Navigators
{
    [Service(ServiceLifetime.Singleton)]
    class NavigatorsService(ILogger<INavigatorsService> logger, IServiceScopeFactory serviceScopeFactory, IRoomsService roomsService) : INavigatorsService, IStartableService
    {
        INavigatorsService Instance => this;

        ConcurrentDictionary<string, IFilter> INavigatorsService.Filters { get; } = [];

        ConcurrentDictionary<int, NavigatorCategory> INavigatorsService.NavigatorCategories { get; } = [];

        ConcurrentDictionary<int, NavigatorPublicCategory> INavigatorsService.PublicCategories { get; } = [];

        async Task IStartableService.StartAsync()
        {
            Instance.PublicCategories.Clear();
            Instance.NavigatorCategories.Clear();
            Instance.Filters.Clear();

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            try
            {
                var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

                var filters = scope.ServiceProvider.GetRequiredService<IEnumerable<IFilter>>().ToList();

                var publicCategories = await mysqlDbContext.NavigatorPublicCategories.Where(npc => npc.Visible).AsNoTracking().ToListAsync();
                publicCategories.ForEach(category => Instance.PublicCategories.TryAdd(category.Id, category.Map()));

                var userCategories = await mysqlDbContext.NavigatorUserCategories.Where(nca => nca.Enabled).AsNoTracking().ToListAsync();
                userCategories.ForEach(category => Instance.NavigatorCategories.TryAdd(category.Id, category.Map()));

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

                filters.ForEach(filter => Instance.Filters.TryAdd(filter.Name, filter));

                logger.LogInformation("NavigatorsService started with {count} filters definitions", Instance.Filters.Count);
                logger.LogInformation("NavigatorsService started with {count} public categories definitions", Instance.PublicCategories.Count);
                logger.LogInformation("NavigatorsService started with {count} user categories definitions", Instance.NavigatorCategories.Count);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Exception during starting of NavigatorsService: {ex}", ex);
            }
            finally
            {
                await scope.DisposeAsync();
            }
        }
    }
}