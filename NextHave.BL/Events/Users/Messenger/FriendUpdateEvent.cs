using NextHave.BL.Clients;
using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Events.Users.Messenger
{
    public class FriendUpdateEvent : UserEvent
    {
        public int FriendId { get; set; }

        public bool Notification { get; set; }

        public Client? Client { get; set; }

        public IRoomInstance? RoomInstance { get; set; }
    }
}