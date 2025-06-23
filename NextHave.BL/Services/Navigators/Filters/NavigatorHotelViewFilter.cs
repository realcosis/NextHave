using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Enums;
using NextHave.BL.Models.Navigators;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Rooms;

namespace Dolphin.HabboHotel.Navigators.Filters
{
    [Service(ServiceLifetime.Singleton)]
    class NavigatorHotelViewFilter(IRoomsService roomsManager) : IFilter
    {
        string IFilter.Name => "hotel_view";

        async Task<List<SearchResultList>> IFilter.GetSearchResults(User habbo, string? _)
        {
            var resultLists = new List<SearchResultList>();

            var popularCategory = new SearchResultList(0, "popular", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, [.. roomsManager.ActiveRooms.OrderByDescending(ar => ar.UsersNow)], false, false, DisplayOrder.ACTIVITY, 1);
            resultLists.Add(popularCategory);

            return await Task.FromResult(resultLists);
        }
    }
}