using NextHave.BL.Messages.Input.Handshake;

namespace NextHave.BL.Messages.Parsers
{
    public class SSOTicketMessageParser : AbstractParser<SSOTicketMessage>
    {
        public override IInput Parse(ClientMessage packet)
            => new SSOTicketMessage
            {
                SSO = packet.ReadString(),
                ElapsedMilliseconds = packet.ReadInt()
            };
    }
}