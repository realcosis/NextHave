using NextHave.BL.Models.Navigators;
using NextHave.BL.Services.Users.Instances;

namespace NextHave.BL.Services.Navigators.Filters
{
    public interface IFilter
    {
        string Name { get; }

        Task<List<SearchResultList>> GetSearchResults(IUserInstance userInstance, string? query);
    }
}