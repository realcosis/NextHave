using NextHave.BL.Messages.Input.Handshake;

namespace NextHave.BL.Messages.Parsers.Handshake
{
    public class SSOTicketMessageParser : AbstractParser<SSOTicketMessage>
    {
        public sealed override IInput Parse(ClientMessage packet)
            => new SSOTicketMessage
            {
                SSO = packet.ReadString(),
                ElapsedMilliseconds = packet.ReadInt()
            };
    }
}