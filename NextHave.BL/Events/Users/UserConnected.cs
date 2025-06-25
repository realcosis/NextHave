using Dolphin.Core.Events;

namespace NextHave.BL.Events.Users
{
    public class UserConnected : DolphinEvent
    {
        public int? UserId { get; set; }
    }
}