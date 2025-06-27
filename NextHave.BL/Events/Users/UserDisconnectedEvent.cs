using Dolphin.Core.Events;

namespace NextHave.BL.Events.Users
{
    public class UserDisconnectedEvent : DolphinEvent
    {
        public int? UserId { get; set; }
    }
}