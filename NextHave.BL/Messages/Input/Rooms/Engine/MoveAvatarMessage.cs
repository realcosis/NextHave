namespace NextHave.BL.Messages.Input.Rooms.Engine
{
    public record MoveAvatarMessage : IInput
    {
        public int NewX { get; init; }

        public int NewY { get; init; }
    }
}