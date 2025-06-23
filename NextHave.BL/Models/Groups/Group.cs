using NextHave.BL.Models.Rooms;
using System.Collections.Concurrent;

namespace NextHave.BL.Models.Groups
{
    public class Group
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int OwnerId { get; set; }

        public string? Description { get; set; }

        public int? RoomId { get; set; }

        public string? Image { get; set; }

        public int CustomColorPrimary { get; set; }

        public int CustomColorSecondary { get; set; }

        public int Base { get; set; }

        public int BaseColor { get; set; }

        public string HtmlColorPrimary { get; set; } = "ffffff";

        public string? HtmlColorSecondary { get; set; } = "ffffff";

        public string? DateCreate { get; set; }

        public string Petitions { get; set; } = "";

        public int Type { get; set; } = 0;

        public int RightsType { get; set; } = 0;

        public int WhoCanRead { get; set; } = 0;

        public int WhoCanPost { get; set; } = 1;

        public int WhoCanThread { get; set; } = 1;

        public int WhoCanMod { get; set; } = 2;

        public int ForumMessagesCount { get; set; } = 0;

        public string ForumScore { get; set; } = "0";

        public int ForumLastPosterId { get; set; } = 0;

        public string ForumLastPosterName { get; set; } = "";

        public int ForumLastPosterTimestamp { get; set; } = 0;

        public int MemberCount { get; set; } = 0;

        public int RequestCount { get; set; } = 0;

        public Room? Room { get; set; }

        public ConcurrentDictionary<int, GroupMember> Members { get; set; } = [];

        public ConcurrentDictionary<int, GroupMember> Requests { get; set; } = [];

        public bool HasRequest(int userId)
            => Requests.ContainsKey(userId);

        public bool IsMember(int userId)
            => Members.ContainsKey(userId);

        public bool IsAdmin(int userId)
            => Members.ContainsKey(userId) && Members.TryGetValue(userId, out var member) && member.Rank < 2;
    }
}