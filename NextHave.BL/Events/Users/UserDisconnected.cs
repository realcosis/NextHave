using Dolphin.Core.Events;

namespace NextHave.BL.Events.Users
{
    public class UserDisconnected : DolphinEvent
    {
        public int? UserId { get; set; }
    }
}