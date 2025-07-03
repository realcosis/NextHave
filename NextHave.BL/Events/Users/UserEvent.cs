using Dolphin.Core.Events;

namespace NextHave.BL.Events.Users
{
    public class UserEvent : DolphinEvent
    {
        public int UserId { get; set; }
    }
}