namespace NextHave.BL.Events.Rooms.Chat
{
    public class ChatMessageEvent : RoomEvent
    {
        public string? Message { get; set; }

        public int UserId { get; set; }

        public int Color { get; set; }
    }
}