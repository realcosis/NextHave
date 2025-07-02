using NextHave.BL.Messages.Input.Rooms.Engine;

namespace NextHave.BL.Messages.Parsers.Rooms
{
    public class MoveObjectParser : AbstractParser<MoveObjectMessage>
    {
        public override IInput Parse(ClientMessage packet)
            => new MoveObjectMessage()
            {
                ItemId = packet.ReadInt(),
                NewX = packet.ReadInt(),
                NewY = packet.ReadInt(),
                Rotation = packet.ReadInt()
            };
    }
}