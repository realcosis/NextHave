namespace NextHave.BL.Messages.Input.Navigators
{
    public record NewNavigatorSearchMessage : IInput
    {
        public string? View { get; init; }

        public string? Query { get; init; }
    }
}