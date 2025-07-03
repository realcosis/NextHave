using NextHave.BL.Services.Users.Instances;

namespace NextHave.BL.Events.Rooms.Users
{
    public class AddUserToRoomEvent : RoomEvent
    {
        public IUserInstance? User { get; set; }

        public bool Spectator { get; set; }
    }
}