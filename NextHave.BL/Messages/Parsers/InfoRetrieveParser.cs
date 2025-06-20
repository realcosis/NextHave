using NextHave.BL.Messages.Input.Handshake;

namespace NextHave.BL.Messages.Parsers
{
    public class InfoRetrieveParser : AbstractParser<InfoRetrieveMessage>
    {
        public override IMessageEvent Parse(ClientMessage packet)
            => new InfoRetrieveMessage();
    }
}