namespace NextHave.BL.Messages.Input.Rooms.Engine
{
    public record MoveObjectMessage : IInput
    {
        public int ItemId { get; init; }

        public int NewX { get; init; }

        public int NewY { get; init; }

        public int Rotation { get; init; }
    }
}