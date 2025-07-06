namespace NextHave.BL.Messages.Input.Navigators
{
    public record GetGuestRoomMessage : IInput
    {
        public int RoomId { get; init; }

        public bool IsLoading { get; init; }

        public bool CheckEntry { get; init; }
    }
}