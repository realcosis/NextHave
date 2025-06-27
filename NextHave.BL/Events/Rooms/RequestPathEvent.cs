using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Events.Rooms
{
    public class RequestPathEvent : RoomEvent
    {
        public int? NewX { get; set; }

        public int? NewY { get; set; }

        public IRoomUserInstance? RoomUserInstance { get; set; }
    }
}