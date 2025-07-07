using NextHave.BL.Messages.Input.Friends;

namespace NextHave.BL.Messages.Parsers.Friends
{
    public class SendMessageParser : AbstractParser<SendMessageMessage>
    {
        public sealed override IInput Parse(ClientMessage packet)
            => new SendMessageMessage
            {
                UserId = packet.ReadInt(),
                Message = packet.ReadString()
            };
    }
}