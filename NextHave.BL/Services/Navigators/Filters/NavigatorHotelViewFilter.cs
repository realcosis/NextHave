using NextHave.BL.Enums;
using Dolphin.Core.Injection;
using NextHave.BL.Services.Rooms;
using NextHave.BL.Models.Navigators;
using NextHave.BL.Services.Users.Instances;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Navigators.Filters
{
    [Service(ServiceLifetime.Singleton)]
    class NavigatorHotelViewFilter(IServiceProvider serviceProvider) : IFilter
    {
        string IFilter.Name => "hotel_view";

        async Task<List<SearchResultList>> IFilter.GetSearchResults(IUserInstance userInstance, string? _)
        {
            var roomsService = serviceProvider.GetRequiredService<IRoomsService>();

            var resultLists = new List<SearchResultList>();

            var popularCategory = new SearchResultList(0, "popular", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, [.. roomsService.ActiveRooms.Values.OrderByDescending(ar => ar.Room.UsersNow).Select(ar => ar.Room)], false, false, DisplayOrder.ACTIVITY, 1);
            resultLists.Add(popularCategory);

            return await Task.FromResult(resultLists);
        }
    }
}