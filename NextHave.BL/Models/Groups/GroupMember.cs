namespace NextHave.BL.Models.Groups
{
    public class GroupMember
    {
        public int Rank { get; set; }

        public DateTime EnteredAt { get; set; }

        public int UserId { get; set; }

        public string? Username { get; set; }

        public string? Look { get; set; }
    }
}