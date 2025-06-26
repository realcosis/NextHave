using NextHave.BL.Messages.Input.Rooms;

namespace NextHave.BL.Messages.Parsers.Rooms
{
    public class GetFurnitureAliasesParser : AbstractParser<GetFurnitureAliasesMessage>
    {
        public override IInput Parse(ClientMessage packet)
            => new GetFurnitureAliasesMessage();
    }
}