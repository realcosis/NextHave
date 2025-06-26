namespace NextHave.BL.Messages.Input.Rooms
{
    public record OpenFlatMessage : IInput
    {
        public int RoomId { get; init; }

        public string? Password { get; init; }
    }
}