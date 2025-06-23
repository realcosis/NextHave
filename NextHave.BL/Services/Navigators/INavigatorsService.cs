using Dolphin.HabboHotel.Navigators.Filters;
using NextHave.BL.Models.Navigators;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Navigators
{
    public interface INavigatorsService
    {
        ConcurrentDictionary<string, IFilter?> Filters { get; }

        ConcurrentDictionary<int, NavigatorPublicCategory> PublicCategories { get; }
    }
}