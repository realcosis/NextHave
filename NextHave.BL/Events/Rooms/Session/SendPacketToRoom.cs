using NextHave.BL.Messages;

namespace NextHave.BL.Events.Rooms.Session
{
    public class SendPacketToRoom : RoomEvent
    {
        public Composer? Composer { get; set; }
    }
}