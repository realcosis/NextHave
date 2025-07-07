using NextHave.BL.Clients;
namespace NextHave.BL.Events.Users.Messenger
{
    public class FriendUpdateEvent : UserEvent
    {
        public int FriendId { get; set; }

        public bool Notification { get; set; }

        public Client? Client { get; set; }
    }
}