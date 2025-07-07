namespace NextHave.BL.Messages.Input.Friends
{
    public record SendMessageMessage : IInput
    {
        public int UserId { get; init; }

        public string? Message { get; init; }
    }
}