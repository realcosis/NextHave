namespace NextHave.Messages
{
    public static class ServerMessageFactory
    {
        public static ServerMessage GetServerMessage(short header)
        {
            var message = new ServerMessage(header);
            message.Init();
            return message;
        }
    }
}