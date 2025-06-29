namespace NextHave.BL.Events.Rooms.Users.Movements
{
    public class MoveAvatarEvent : RoomEvent
    {
        public int? UserId { get; set; }

        public int? NewX { get; set; }

        public int? NewY { get; set; }
    }
}