using NextHave.BL.Messages.Input.Rooms.Engine;

namespace NextHave.BL.Messages.Parsers.Rooms.Rooms
{
    public class MoveAvatarParser : AbstractParser<MoveAvatarMessage>
    {
        public override IInput Parse(ClientMessage packet)
            => new MoveAvatarMessage()
            {
                NewX = packet.ReadInt(),
                NewY = packet.ReadInt()
            };
    }
}