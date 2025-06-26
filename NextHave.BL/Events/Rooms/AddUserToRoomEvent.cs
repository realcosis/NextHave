using NextHave.BL.Models.Users;

namespace NextHave.BL.Events.Rooms
{
    public class AddUserToRoomEvent : RoomEvent
    {
        public User? User { get; set; }

        public bool Spectator { get; set; }
    }
}