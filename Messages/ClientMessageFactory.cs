using System.Collections.Concurrent;

namespace NextHave.Messages
{
    public static class ClientMessageFactory
    {
        public static ClientMessage GetClientMessage(byte[] body, int position, string sessionId)
            => new(body, position, sessionId);
    }
}