using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Enums;
using NextHave.BL.Models.Navigators;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Navigators;

namespace Dolphin.HabboHotel.Navigators.Filters
{
    [Service(ServiceLifetime.Singleton)]
    class NavigatorOfficialFilter(INavigatorsService navigatorManager) : IFilter
    {
        string IFilter.Name => "official_view";

        async Task<List<SearchResultList>> IFilter.GetSearchResults(User habbo, string? _)
        {
            var resultLists = new List<SearchResultList>();
            var i = 0;

            foreach (var category in navigatorManager.PublicCategories.Values)
            {
                if (category.Rooms.Count != 0)
                {
                    resultLists.Add(new SearchResultList(i, "", category.Name!, SearchAction.NONE, ListMode.THUMBNAILS, DisplayMode.VISIBLE, category.Rooms, true, false, DisplayOrder.ORDER_NUM, category.OrderNum));
                    i++;
                }
            }

            return await Task.FromResult(resultLists);
        }
    }
}