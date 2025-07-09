using NextHave.DAL.Enums;

namespace NextHave.BL.Messages.Input.Rooms.Settings
{
    public class SaveRoomSettingsMessage : IInput
    {
        public int RoomId { get; init; }

        public string? Name { get; init; }

        public string? Description { get; init; }

        public RoomAccessStatus State { get; init; }

        public string? Password { get; init; }

        public int UsersMax { get; init; }

        public int CategoryId { get; init; }

        public List<string> Tags { get; init; } = [];

        public int TradeType { get; init; }

        public bool AllowPets { get; init; }

        public bool AllowPetsEat { get; init; }

        public bool AllowWalkthrough { get; init; }

        public bool AllowHideWall { get; init; }

        public int WallThickness { get; init; }

        public int FloorThickness { get; init; }

        public int MuteType { get; init; }

        public int KickType { get; init; }

        public int BanType { get; init; }

        public int ChatModeType { get; init; }

        public int ChatWeightType { get; init; }

        public int ChatSpeed { get; init; }

        public int ChatDistance { get; init; }

        public int ChatProtectionType { get; init; }

        public bool AllowNavigatorDynamicCategories { get; init; }
    }
}