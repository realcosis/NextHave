namespace NextHave.BL.Events.Rooms.Users.Movements
{
    public class ProcessMovementEvent : RoomEvent
    {
        public int VirtualId { get; set; }

        public DateTime RequestTime { get; set; }
    }
}