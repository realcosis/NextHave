using NextHave.BL.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Parsers
{
    public interface IPacketParser : IDisposable
    {
        event Func<ClientMessage, IServiceScopeFactory, short, Task>? OnNewPacket;

        void HandlePacketData(byte[] packet, int bytesReceived, string sessionId);
    }
}