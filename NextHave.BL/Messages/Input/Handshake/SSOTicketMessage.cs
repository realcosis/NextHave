﻿namespace NextHave.BL.Messages.Input.Handshake
{
    public record SSOTicketMessage : IMessageEvent
    {
        public string? SSO { get; init; }

        public int ElapsedMilliseconds { get; init; }
    }
}