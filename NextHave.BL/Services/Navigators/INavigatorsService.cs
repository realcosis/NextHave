using NextHave.BL.Models.Navigators;
using System.Collections.Concurrent;
using NextHave.BL.Models.Rooms.Navigators;
using NextHave.BL.Services.Navigators.Filters;

namespace NextHave.BL.Services.Navigators
{
    public interface INavigatorsService
    {
        ConcurrentDictionary<string, IFilter> Filters { get; }

        ConcurrentDictionary<int, NavigatorPublicCategory> PublicCategories { get; }

        ConcurrentDictionary<int, NavigatorCategory> NavigatorCategories { get; }
    }
}