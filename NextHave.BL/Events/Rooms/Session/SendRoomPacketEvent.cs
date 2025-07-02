using NextHave.BL.Messages;

namespace NextHave.BL.Events.Rooms.Session
{
    public class SendRoomPacketEvent : RoomEvent
    {
        public Composer? Composer { get; set; }

        public bool WithRights { get; set; } = false;
    }
}