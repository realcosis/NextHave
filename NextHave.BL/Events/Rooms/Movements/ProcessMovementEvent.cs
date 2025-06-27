namespace NextHave.BL.Events.Rooms.Movements
{
    public class ProcessMovementEvent : RoomEvent
    {
        public int VirtualId { get; set; }

        public DateTime RequestTime { get; set; }
    }
}