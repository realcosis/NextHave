using NextHave.BL.Messages.Input.Rooms;

namespace NextHave.BL.Messages.Parsers.Rooms
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