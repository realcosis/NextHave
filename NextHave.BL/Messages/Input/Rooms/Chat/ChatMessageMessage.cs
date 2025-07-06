namespace NextHave.BL.Messages.Input.Rooms.Chat
{
    public record ChatMessageMessage : IInput
    {
        public string? Message { get; init; }

        public int Color { get; init; }
    }
}