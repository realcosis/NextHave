using NextHave.BL.Models.Navigators;
using NextHave.BL.Models.Users;

namespace Dolphin.HabboHotel.Navigators.Filters
{
    public interface IFilter
    {
        string Name { get; }

        Task<List<SearchResultList>> GetSearchResults(User habbo, string? query);
    }
}