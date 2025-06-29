using NextHave.DAL.Enums;
using NextHave.BL.Messages;
using NextHave.BL.Models.Groups;

namespace NextHave.BL.Models.Rooms
{
    public class Room
    {
        public int Id { get; set; }

        public string? RoomType { get; set; } = "private";

        public string? Caption { get; set; } = "Room";

        public int? OwnerId { get; set; }

        public string? Owner { get; set; }

        public string? Description { get; set; }

        public int Category { get; set; } = 0;

        public RoomAccessStatus? State { get; set; } = RoomAccessStatus.Open;

        public int UsersNow { get; set; } = 0;

        public int UsersMax { get; set; } = 25;

        public string? ModelName { get; set; }

        public int Score { get; set; } = 0;

        public string? Tags { get; set; } = "";

        public int IconBg { get; set; } = 1;

        public int IconFg { get; set; } = 0;

        public string? IconItems { get; set; } = "0.0";

        public string? Password { get; set; } = "";

        public string? Wallpaper { get; set; } = "0.0";

        public string? Floor { get; set; } = "0.0";

        public string? Landscape { get; set; } = "0.0";

        public bool AllowPets { get; set; }

        public bool AllowPetsEat { get; set; }

        public bool AllowWalkthrough { get; set; }

        public bool AllowHideWall { get; set; }

        public bool AllowRightsOverride { get; set; }

        public bool AllowDiagonal { get; set; }

        public int WallThickness { get; set; } = 0;

        public int FloorThickness { get; set; } = 0;

        public int GroupId { get; set; } = 0;

        public int? MuteSettings { get; set; } = 1;

        public int? BanSettings { get; set; } = 1;

        public int KickSettings { get; set; } = 1;

        public int BubbleOption { get; set; } = 1;

        public int BubbleSize { get; set; } = 0;

        public int Displacement { get; set; } = 1;

        public int DistanceListen { get; set; } = 14;

        public string? TradeState { get; set; }

        public int? WallHeight { get; set; } = 2;

        public int TradeSettings { get; set; } = 2;

        public int RollerSpeed { get; set; } = 4;

        public string HideWired { get; set; } = "0";

        public Group? Group { get; set; }

        public void Serialize(ServerMessage message)
        {
            message.AddInt32(Id);
            message.AddString(Caption!);

            if (RoomType == "public")
            {
                message.AddInt32(0);
                message.AddString("");
            }
            else
            {
                message.AddInt32(OwnerId!.Value);
                message.AddString(Owner!);
            }

            message.AddInt32(1);
            message.AddInt32(UsersNow);
            message.AddInt32(UsersMax);
            message.AddString(Description!);
            message.AddInt32(TradeSettings);
            message.AddInt32(Score);
            message.AddInt32(0);
            message.AddInt32(Category);

            var tags = Tags?.Split(";").ToList() ?? [];
            message.AddInt32(tags.Count);
            foreach (var tag in tags)
            {
                message.AddString(tag);
            }

            int maskBase = 0;

            if (Group != default)
            {
                maskBase |= 2;
            }

            if (RoomType == "private")
            {
                maskBase |= 8;
            }

            if (AllowPets)
            {
                maskBase |= 16;
            }

            message.AddInt32(maskBase);

            if (Group != default)
            {
                message.AddInt32(Group.Id);
                message.AddString(Group.Name!);
                message.AddString(Group.Image!);
            }
        }
    }
}