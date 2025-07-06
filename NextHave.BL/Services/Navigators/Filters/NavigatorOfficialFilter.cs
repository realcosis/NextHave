using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Enums;
using NextHave.BL.Models.Navigators;
using NextHave.BL.Services.Rooms;
using NextHave.BL.Services.Users.Instances;

namespace NextHave.BL.Services.Navigators.Filters
{
    [Service(ServiceLifetime.Singleton)]
    class NavigatorOfficialFilter(IServiceScopeFactory serviceScopeFactory) : IFilter
    {
        string IFilter.Name => "official_view";

        async Task<List<SearchResultList>> IFilter.GetSearchResults(IUserInstance userInstance, string? _)
        {           
            var resultLists = new List<SearchResultList>();
            
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var navigatorsService = scope.ServiceProvider.GetRequiredService<INavigatorsService>();
            var roomsService = scope.ServiceProvider.GetRequiredService<IRoomsService>();
            await scope.DisposeAsync();

            foreach (var (index, category) in navigatorsService.PublicCategories.Values.Index())
            {
                if (category.Rooms.Count != 0)
                {
                    category.Rooms.ForEach(r => r.UsersNow = roomsService.ActiveRooms.TryGetValue(r.Id, out var room) ? room.Room!.UsersNow : 0);
                    resultLists.Add(new SearchResultList(index, string.Empty, category.Name!, SearchAction.NONE, ListMode.THUMBNAILS, DisplayMode.VISIBLE, category.Rooms, true, false, DisplayOrder.ORDER_NUM, category.OrderNumber));
                }
            }

            return await Task.FromResult(resultLists);
        }
    }
}