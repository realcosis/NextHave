namespace NextHave.BL.Events.Rooms.Chat
{
    public class GetUserVirtualIdEvent : RoomEvent
    {
        public int UserId { get; set; }

        public string? Type { get; set; }
    }
}