using NextHave.BL.Messages.Input.Rooms.Connection;

namespace NextHave.BL.Messages.Parsers.Rooms.Connection
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