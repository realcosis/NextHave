using NextHave.BL.Messages.Input.Navigators;

namespace NextHave.BL.Messages.Parsers.Navigators
{
    public class NewNavigatorSearchParser : AbstractParser<NewNavigatorSearchMessage>
    {
        public sealed override IInput Parse(ClientMessage packet)
        {
            var view = packet.ReadString();
            var query = packet.ReadString();

            if (!string.IsNullOrWhiteSpace(query))
                view = "query";

            return new NewNavigatorSearchMessage
            {
                View = view,
                Query = query
            };
        }
    }
}