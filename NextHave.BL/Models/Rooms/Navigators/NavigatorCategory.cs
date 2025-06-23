namespace NextHave.BL.Models.Rooms.Navigators
{
    public class NavigatorCategory
    {
        public int Id { get; set; }

        public string? Caption { get; set; }

        public string Enabled { get; set; } = "1";

        public int MinRank { get; set; } = 1;
    }
}