using NextHave.Messages;

namespace NextHave.Parsers
{
    public interface IParser : IDisposable
    {
        event Func<ClientMessage, short, Task>? OnNewPacket;

        void HandlePacketData(byte[] packet, int bytesReceived, string sessionId);
    }
}