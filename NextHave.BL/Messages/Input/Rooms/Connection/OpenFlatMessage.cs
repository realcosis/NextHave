namespace NextHave.BL.Messages.Input.Rooms.Connection
{
    public record OpenFlatMessage : IInput
    {
        public int RoomId { get; init; }

        public string? Password { get; init; }
    }
}