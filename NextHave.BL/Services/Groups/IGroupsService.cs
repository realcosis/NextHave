using NextHave.BL.Models.Groups;
using System.Collections.Concurrent;

namespace Dolphin.HabboHotel.Groups
{
    public interface IGroupsService
    {
        ConcurrentDictionary<string, GroupElement> GroupElements { get; }

        Task<Group?> GetGroup(int groupId);
    }
}