using NextHave.BL.Utils;
using NextHave.BL.Messages.Input.Rooms.Chat;

namespace NextHave.BL.Messages.Parsers.Rooms.Chat
{
    public class ShoutMessageParser : AbstractParser<ShoutMessageMessage>
    {
        public override IInput Parse(ClientMessage packet)
            => new ShoutMessageMessage()
            {
                Message = packet.ReadString().Escape(),
                Color = packet.ReadInt()
            };
    }
}