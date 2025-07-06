using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Enums;
using NextHave.BL.Models.Navigators;
using NextHave.BL.Services.Navigators.Filters;
using NextHave.BL.Services.Rooms;
using NextHave.BL.Services.Users.Instances;

namespace Dolphin.HabboHotel.Navigators.Filters
{
    [Service(ServiceLifetime.Singleton)]
    public class NavigatorMyWorldFilter(IServiceScopeFactory serviceScopeFactory) : IFilter
    {
        string IFilter.Name => "myworld_view";

        async Task<List<SearchResultList>> IFilter.GetSearchResults(IUserInstance userInstance, string? _)
        {
            if (userInstance?.User == default)
                return [];

            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var roomsService = scope.ServiceProvider.GetRequiredService<IRoomsService>();
            await scope.DisposeAsync();

            var resultLists = new List<SearchResultList>();

            userInstance.User!.Rooms.ForEach(r => r.UsersNow = roomsService.ActiveRooms.TryGetValue(r.Id, out var room) ? room.Room!.UsersNow : 0);
            var myRooms = new SearchResultList(0, "my", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, [.. userInstance.User!.Rooms.OrderByDescending(ar => ar.UsersNow)], false, false, DisplayOrder.ACTIVITY, 1);
            resultLists.Add(myRooms);

            userInstance.User!.FavoriteRooms.ForEach(r => r.UsersNow = roomsService.ActiveRooms.TryGetValue(r.Id, out var room) ? room.Room!.UsersNow : 0);
            var favoritesRooms = new SearchResultList(1, "favorites", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, [.. userInstance.User.FavoriteRooms.OrderByDescending(fr => fr.UsersNow)], false, false, DisplayOrder.ACTIVITY, 2);
            resultLists.Add(favoritesRooms);

            userInstance.User!.Groups.Where(g => g.Room != default).Select(g => g.Room!).ToList().ForEach(r => r.UsersNow = roomsService.ActiveRooms.TryGetValue(r.Id, out var room) ? room.Room!.UsersNow : 0);
            var myGroups = new SearchResultList(2, "my_groups", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, [.. userInstance.User.Groups.Where(g => g.Room != default).Select(g => g.Room!).OrderByDescending(gr => gr.UsersNow)], false, false, DisplayOrder.ACTIVITY, 3);
            resultLists.Add(myGroups);

            return await Task.FromResult(resultLists);
        }
    }
}