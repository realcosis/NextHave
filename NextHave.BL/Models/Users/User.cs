using NextHave.BL.Models.Groups;
using NextHave.BL.Models.Rooms;

namespace NextHave.BL.Models.Users
{
    public class User
    {
        public int Id { get; set; }

        public string? Username { get; set; }

        public DateTime? LastOnline { get; set; }

        public string? Motto { get; set; }

        public bool IsOnline { get; set; }

        public string? Look { get; set; }

        public int Rank { get; set; }

        public string? Gender { get; set; }

        public int? HomeRoom { get; set; }

        public List<Room> FavoriteRooms { get; set; } = [];

        public List<Room> Rooms { get; set; } = [];

        public List<Group> Groups { get; set; } = [];
    }
}