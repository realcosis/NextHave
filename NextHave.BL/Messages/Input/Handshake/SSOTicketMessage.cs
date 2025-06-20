namespace NextHave.BL.Messages.Input.Handshake
{
    public record SSOTicketMessage : IInput
    {
        public string? SSO { get; init; }

        public int ElapsedMilliseconds { get; init; }
    }
}