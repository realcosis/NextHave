using NextHave.BL.Utils;
using NextHave.BL.Messages.Input.Rooms.Chat;

namespace NextHave.BL.Messages.Parsers.Rooms.Chat
{
    public class ChatMessageParser : AbstractParser<ChatMessageMessage>
    {
        public sealed override IInput Parse(ClientMessage packet)
            => new ChatMessageMessage()
            {
                Message = packet.ReadString().Escape(),
                Color = packet.ReadInt()
            };
    }
}