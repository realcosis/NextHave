using NextHave.BL.Messages.Input.Navigators;

namespace NextHave.BL.Messages.Parsers.Navigators
{
    public class CreateFlatParser : AbstractParser<CreateFlatMessage>
    {
        public override IInput Parse(ClientMessage packet)
            => new CreateFlatMessage
            {
                Name = packet.ReadString(),
                Description = packet.ReadString(),
                ModelName = packet.ReadString(),
                CategoryId = packet.ReadInt(),
                MaxPlayers = packet.ReadInt(),
                TradeType = packet.ReadInt()
            };
    }
}