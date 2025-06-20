using NextHave.DAL.Enums;

namespace NextHave.DAL.Utils
{
    public static class RoomAccessStatusUtilities
    {
        public static int? ToInt(this RoomAccessStatus accessStatus)
            => accessStatus switch
            {
                RoomAccessStatus.Invisible => 3,
                RoomAccessStatus.Password => 2,
                RoomAccessStatus.Locked => 1,
                _ => 0
            };
    }
}