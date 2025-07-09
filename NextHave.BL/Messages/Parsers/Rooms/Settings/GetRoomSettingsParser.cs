using NextHave.BL.Messages.Input.Rooms.Settings;

namespace NextHave.BL.Messages.Parsers.Rooms.Settings
{
    public class GetRoomSettingsParser : AbstractParser<GetRoomSettingsMessage>
    {
        public override GetRoomSettingsMessage Parse(ClientMessage packet)
            => new()
            {
                RoomId = packet.ReadInt()
            };
    }
}