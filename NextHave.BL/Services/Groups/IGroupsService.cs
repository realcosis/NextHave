using NextHave.BL.Models.Groups;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Groups
{
    public interface IGroupsService
    {
        ConcurrentDictionary<string, GroupElement> GroupElements { get; }

        Task<Group?> GetGroup(int groupId);
    }
}