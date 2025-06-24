using NextHave.BL.Messages;

namespace NextHave.BL.PacketParsers
{
    public interface IPacketParser : IDisposable
    {
        ClientMessage? HandlePacket(byte[] packet, int bytesReceived, string sessionId);
    }
}