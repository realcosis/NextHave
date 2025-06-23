namespace NextHave.BL.Messages
{
    public static class ClientMessageFactory
    {
        public static ClientMessage GetClientMessage(byte[] body, int position, string sessionId, short header)
            => new(body, position, sessionId, header);
    }
}