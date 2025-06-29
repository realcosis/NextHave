using NextHave.BL.Models;

namespace NextHave.BL.Events.Rooms.Users.Movements
{
    public class MovementCompleteEvent : RoomEvent
    {
        public int VirtualId { get; set; }

        public Point? Position { get; set; }
    }
}