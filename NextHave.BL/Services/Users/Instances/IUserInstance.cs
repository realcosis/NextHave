using NextHave.BL.Clients;
using NextHave.BL.Models.Permissions;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Rooms;
using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Services.Users.Instances
{
    public interface IUserInstance
    {
        public Client? Client { get; set; }

        public User? User { get; set; }

        public UserEventsService EventsService { get; }

        public int? CurrentRoomId { get; set; }

        public IRoomInstance? CurrentRoomInstance { get; set; }

        public Permission? Permission { get; set; }

        public bool IsMuted { get; set; }

        public DateTime? MutedUntil { get; set; }

        Task Init();
    }
}