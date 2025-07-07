namespace NextHave.BL.Events.Users.Friends
{
    public class SendMessageEvent : UserEvent
    {
        public int UserToId { get; set; }

        public string? Message { get; set; }
    }
}