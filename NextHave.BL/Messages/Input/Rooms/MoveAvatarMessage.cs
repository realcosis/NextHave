namespace NextHave.BL.Messages.Input.Rooms
{
    public record MoveAvatarMessage : IInput
    {
        public int NewX { get; init; }

        public int NewY { get; init; }
    }
}