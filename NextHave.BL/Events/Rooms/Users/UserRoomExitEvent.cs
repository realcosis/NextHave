namespace NextHave.BL.Events.Rooms.Users
{
    public class UserRoomExitEvent : RoomEvent
    {
        public int UserId { get; set; }
    }
}