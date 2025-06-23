using NextHave.BL.Messages;

namespace NextHave.BL.Parsers
{
    public interface IPacketParser : IDisposable
    {
        void HandlePacketData(byte[] packet, int bytesReceived, string sessionId);

        ClientMessage? ManagePacket(byte[] packet, int bytesReceived, string sessionId);
    }
}