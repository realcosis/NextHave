using NextHave.BL.Models;
using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Events.Rooms
{
    public class PathCalculatedEvent : RoomEvent
    {
        public IRoomUserInstance? RoomUserInstance { get; set; }

        public Point? Point { get; set; }
    }
}