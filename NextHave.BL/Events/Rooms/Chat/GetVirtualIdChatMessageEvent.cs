namespace NextHave.BL.Events.Rooms.Chat
{
    public class GetVirtualIdChatMessageEvent : RoomEvent
    {
        public string? Message { get; set; }

        public int Color { get; set; }

        public int UserId { get; set; }
    }
}