using Dolphin.Core.Events;

namespace NextHave.BL.Events.Rooms
{
    public class RoomEvent : DolphinEvent
    {
        public int RoomId { get; set; }
    }
}