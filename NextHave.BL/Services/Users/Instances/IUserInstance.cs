using NextHave.BL.Clients;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Rooms;
using NextHave.BL.Models.Permissions;
using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Services.Users.Instances
{
    public interface IUserInstance
    {
        Client? Client { get; set; }

        User? User { get; set; }

        UserEventsService EventsService { get; }

        int? CurrentRoomId { get; set; }

        IRoomInstance? CurrentRoomInstance { get; }

        Permission? Permission { get; set; }

        bool IsMuted { get; set; }

        DateTime? MutedUntil { get; set; }

        Task Init();

        Task Dispose();
    }
}