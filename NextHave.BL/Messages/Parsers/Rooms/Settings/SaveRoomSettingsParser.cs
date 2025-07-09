using NextHave.DAL.Utils;
using NextHave.BL.Messages.Input.Rooms.Settings;
using NextHave.DAL.Enums;

namespace NextHave.BL.Messages.Parsers.Rooms.Settings
{
    public class SaveRoomSettingsParser : AbstractParser<SaveRoomSettingsMessage>
    {
        public sealed override IInput Parse(ClientMessage packet)
        {
            var roomId = packet.ReadInt();
            var name = packet.ReadString();
            var description = packet.ReadString();
            var state = packet.ReadInt().ToRoomAccessStatus();
            var password = packet.ReadString();
            var usersMax = packet.ReadInt();
            var categoryId = packet.ReadInt();
            var tags = new List<string>();
            var totalTags = packet.ReadInt();

            if (totalTags > 0)
                while (totalTags > 0)
                {
                    tags.Add(packet.ReadString());
                    totalTags--;
                }

            var tradeMode = packet.ReadInt();
            var allowPets = packet.ReadBool();
            var allowPetsEat = packet.ReadBool();
            var allowWalkthrough = packet.ReadBool();
            var allowHideWall = packet.ReadBool();
            var wallThickness = packet.ReadInt();
            var floorThickness = packet.ReadInt();
            var whoCanMute = packet.ReadInt();
            var whoCanKick = packet.ReadInt();
            var whoCanBan = packet.ReadInt();
            var chatMode = packet.ReadInt();
            var chatWeight = packet.ReadInt();
            var chatSpeed = packet.ReadInt();
            var chatDistance = packet.ReadInt();
            var chatProtection = packet.ReadInt();
            var allowNavigatorDynamicCategories = false;

            if (packet.RemainingLength > 0)
                allowNavigatorDynamicCategories = packet.ReadBool();

            if (chatMode < 0 || chatMode > 1)
                chatMode = 0;

            if (chatWeight < 0 || chatWeight > 2)
                chatWeight = 0;

            if (chatSpeed < 0 || chatSpeed > 2)
                chatSpeed = 0;

            if (chatDistance < 0)
                chatDistance = 1;

            if (chatDistance > 99)
                chatDistance = 100;

            if (chatProtection < 0 || chatProtection > 2)
                chatProtection = 0;

            if (tradeMode < 0 || tradeMode > 2)
                tradeMode = 0;

            if (whoCanMute < 0 || whoCanMute > 1)
                whoCanMute = 0;

            if (whoCanKick < 0 || whoCanKick > 1)
                whoCanKick = 0;

            if (whoCanBan < 0 || whoCanBan > 1)
                whoCanBan = 0;

            if (wallThickness < -2 || wallThickness > 1)
                wallThickness = 0;

            if (floorThickness < -2 || floorThickness > 1)
                floorThickness = 0;

            if (name.Length > 60)
                name = name[..60];

            if (state == RoomAccessStatus.Password && string.IsNullOrWhiteSpace(password))
                state = RoomAccessStatus.Open;

            if (usersMax < 0)
                usersMax = 10;

            if (usersMax > 100)
                usersMax = 100;

            return new SaveRoomSettingsMessage
            {
                RoomId = roomId,
                Name = name,
                Description = description,
                State = state,
                Password = password,
                UsersMax = usersMax,
                CategoryId = categoryId,
                Tags = tags,
                TradeType = tradeMode,
                AllowPets = allowPets,
                AllowPetsEat = allowPetsEat,
                AllowWalkthrough = allowWalkthrough,
                AllowHideWall = allowHideWall,
                WallThickness = wallThickness,
                FloorThickness = floorThickness,
                MuteType = whoCanMute,
                KickType = whoCanKick,
                BanType = whoCanBan,
                ChatModeType = chatMode,
                ChatWeightType = chatWeight,
                ChatSpeed = chatSpeed,
                ChatDistance = chatDistance,
                ChatProtectionType = chatProtection,
                AllowNavigatorDynamicCategories = allowNavigatorDynamicCategories
            };
        }
    }
}