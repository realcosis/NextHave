namespace NextHave.BL.Events.Rooms.Chat
{
    public class GetVirtualIdForChatEvent : RoomEvent
    {
        public string? Message { get; set; }

        public int Color { get; set; }

        public int UserId { get; set; }

        public bool Shout { get; set; }
    }
}