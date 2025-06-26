namespace NextHave.BL.Events.Rooms
{
    public class RoomBanEvent : RoomEvent
    {
        public int UserId { get; set; }

        public double Time { get; set; }
    }
}