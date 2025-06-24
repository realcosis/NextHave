using Dolphin.Core.Injection;
using NextHave.BL.Models.Navigators;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Models.Users;

namespace Dolphin.HabboHotel.Navigators.Filters
{
    [Service(ServiceLifetime.Singleton)]
    class NavigatorMyWorldFilter : IFilter
    {
        string IFilter.Name => "myworld_view";

        async Task<List<SearchResultList>> IFilter.GetSearchResults(User habbo, string? _)
        {
            var resultLists = new List<SearchResultList>();

            //var myRooms = new SearchResultList(0, "my", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, [.. habbo.Rooms.OrderByDescending(ar => ar.UsersNow)], false, false, DisplayOrder.ACTIVITY, 1);
            //resultLists.Add(myRooms);

            //var favoritesRooms = new SearchResultList(1, "favorites", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, [.. habbo.FavoriteRooms.OrderByDescending(fr => fr.UsersNow)], false, false, DisplayOrder.ACTIVITY, 2);
            //resultLists.Add(favoritesRooms);

            //var myGroups = new SearchResultList(2, "my_groups", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, [.. habbo.Groups.Where(g => g.Room != default).Select(g => g.Room!).OrderByDescending(gr => gr.UsersNow)], false, false, DisplayOrder.ACTIVITY, 3);
            //resultLists.Add(myGroups);

            //var friendRooms = new SearchResultList(3, "friends_rooms", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, [], false, false, DisplayOrder.ACTIVITY, 4); // TODO: After room done
            //resultLists.Add(friendRooms);

            // TODO: Room visits

            return await Task.FromResult(resultLists);
        }
    }
}