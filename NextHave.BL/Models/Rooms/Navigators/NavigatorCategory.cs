namespace NextHave.BL.Models.Rooms.Navigators
{
    public class NavigatorCategory
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public bool Enabled { get; set; }

        public int MinRank { get; set; } = 1;
    }
}