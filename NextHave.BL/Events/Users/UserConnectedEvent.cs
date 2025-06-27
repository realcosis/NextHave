using Dolphin.Core.Events;

namespace NextHave.BL.Events.Users
{
    public class UserConnectedEvent : DolphinEvent
    {
        public int? UserId { get; set; }
    }
}