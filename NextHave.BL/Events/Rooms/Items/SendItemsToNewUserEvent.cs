using NextHave.BL.Clients;

namespace NextHave.BL.Events.Rooms.Items
{
    public class SendItemsToNewUserEvent : RoomEvent
    {
        public Client? Client { get; set; }
    }
}