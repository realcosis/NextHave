using NextHave.BL.Clients;
using NextHave.BL.Services.Rooms.Instances;

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

        public Client? Client { get; set; }

        public int? CurrentRoomId { get; set; }

        public IRoomInstance? CurrentRoomInstance { get; set; }
    }
}