namespace NextHave.BL.Messages.Input.Rooms.Settings
{
    public record GetRoomSettingsMessage : IInput
    {
        public int RoomId { get; init; }
    }
}