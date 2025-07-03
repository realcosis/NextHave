using NextHave.BL.Utils;
using NextHave.BL.Messages.Input.Rooms.Chat;

namespace NextHave.BL.Messages.Parsers.Rooms.Chat
{
    public class ChatMessageParser : AbstractParser<ChatMessage>
    {
        public override IInput Parse(ClientMessage packet)
            => new ChatMessage()
            {
                Message = packet.ReadString().Escape(),
                Color = packet.ReadInt()
            };
    }
}