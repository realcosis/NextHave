namespace NextHave.BL.Messages
{
    public static class ClientMessageFactory
    {
        public static ClientMessage GetClientMessage(byte[] body, int position, short header)
            => new(body, position, header);
    }
}