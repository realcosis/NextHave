namespace NextHave.BL.Messages.Input.Navigators
{
    public record CreateFlatMessage : IInput
    {
        public string? Name { get; init; }

        public string? Description { get; init; }

        public string? ModelName { get; init; }

        public int CategoryId { get; init; }

        public int MaxPlayers { get; init; }

        public int TradeType { get; init; }
    }
}