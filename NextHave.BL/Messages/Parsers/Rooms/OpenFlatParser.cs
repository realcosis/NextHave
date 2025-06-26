using NextHave.BL.Messages.Input.Rooms;

namespace NextHave.BL.Messages.Parsers.Rooms
{
    public class OpenFlatParser : AbstractParser<OpenFlatMessage>
    {
        public override IInput Parse(ClientMessage packet)
            => new OpenFlatMessage()
            {
                RoomId = packet.ReadInt(),
                Password = packet.ReadString(),
            };
    }
}