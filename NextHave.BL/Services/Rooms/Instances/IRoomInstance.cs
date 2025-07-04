using NextHave.BL.Models.Rooms;
using NextHave.BL.Services.Users.Instances;

namespace NextHave.BL.Services.Rooms.Instances
{
    public interface IRoomInstance
    {
        Room? Room { get; set; }

        bool RoomMuted { get; set; }

        RoomToner? Toner { get; set; }

        WorkRoomModel? RoomModel { get; set; }

        RoomEventsService EventsService { get; }

        Task OnRoomTick();

        Task Init();

        bool CheckRights(IUserInstance userInstance, bool isOwner);

        void MuteUser(int virtualId, DateTime until);

        bool CheckMute(int virtualId, IUserInstance userInstance);
    }
}