using NextHave.Messages.Input.Handshake;

namespace NextHave.Messages.Parsers
{
    public class InfoRetrieveParser : AbstractParser<InfoRetrieveMessage>
    {
        public override IMessageEvent Parse(ClientMessage packet)
            => new InfoRetrieveMessage();
    }
}