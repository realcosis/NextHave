using System.Collections.Concurrent;

namespace NextHave.Messages
{
    public static class ClientMessageFactory
    {
        readonly static ConcurrentQueue<ClientMessage> messages = [];

        public static ClientMessage GetClientMessage(byte[] body, int position, string sessionId)
            => new(body, position, sessionId);

        public static void Enqueue(ClientMessage message)
            => messages.Enqueue(message);
    }
}