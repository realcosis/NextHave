using NextHave.BL.Models.Rooms;

namespace NextHave.BL.Services.Rooms.Instances
{
    public interface IRoomInstance
    {
        public Room? Room { get; set; }

        public RoomModel? RoomModel { get; set; }

        public RoomEventsService EventsService { get; }

        Task Init();
    }
}