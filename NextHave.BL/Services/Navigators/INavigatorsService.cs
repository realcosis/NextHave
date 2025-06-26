using NextHave.BL.Models.Navigators;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Navigators
{
    public interface INavigatorsService
    {
        ConcurrentDictionary<int, NavigatorPublicCategory> PublicCategories { get; }
    }
}