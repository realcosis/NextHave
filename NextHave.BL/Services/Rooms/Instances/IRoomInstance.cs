using NextHave.BL.Models.Rooms;
using NextHave.BL.Services.Rooms.Pathfinders;

namespace NextHave.BL.Services.Rooms.Instances
{
    public interface IRoomInstance
    {
        public Room? Room { get; set; }

        public WorkRoomModel? RoomModel { get; set; }

        public RoomEventsService EventsService { get; }

        Task OnRoomTick();

        Task Init();
    }
}