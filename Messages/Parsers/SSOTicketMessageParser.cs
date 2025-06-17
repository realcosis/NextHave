using NextHave.Messages.Input.Handshake;

namespace NextHave.Messages.Parsers
{
    public class SSOTicketMessageParser : AbstractParser<SSOTicketMessage>
    {
        public override IMessageEvent Parse(ClientMessage packet)
            => new SSOTicketMessage
            {
                SSO = packet.ReadString(),
                ElapsedMilliseconds = packet.ReadInt()
            };
    }
}