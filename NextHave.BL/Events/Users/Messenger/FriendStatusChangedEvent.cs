namespace NextHave.BL.Events.Users.Messenger
{
    public class FriendStatusChangedEvent : UserEvent
    {
        public bool Notification { get; set; }
    }
}