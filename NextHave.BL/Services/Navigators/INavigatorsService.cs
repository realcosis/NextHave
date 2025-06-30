using NextHave.BL.Models.Navigators;
using System.Collections.Concurrent;
using NextHave.BL.Models.Rooms.Navigators;

namespace NextHave.BL.Services.Navigators
{
    public interface INavigatorsService
    {
        ConcurrentDictionary<int, NavigatorPublicCategory> PublicCategories { get; }

        ConcurrentDictionary<int, NavigatorCategory> NavigatorCategories { get; }
    }
}