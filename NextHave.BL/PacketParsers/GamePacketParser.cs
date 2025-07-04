using DnsClient.Internal;
using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Messages;
using NextHave.BL.Utils;

namespace NextHave.BL.PacketParsers
{
    [Service(ServiceLifetime.Scoped)]
    class GamePacketParser : IPacketParser
    {
        readonly ILogger<GamePacketParser> _logger;

        static readonly int INT_SIZE = 4;

        int currentPacketLength;

        int bufferPos;

        readonly byte[] bufferedData;

        public GamePacketParser(ILogger<GamePacketParser> logger)
        {
            ResetState();
            _logger = logger;
            bufferedData = new byte[4096];
        }

        public ClientMessage? HandlePacket(byte[] data, int bytes, string sessionId)
        {
            var clientMessage = default(ClientMessage?);

            try
            {
                clientMessage = ProcessData(data, bytes);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing packet data for session {sessionId}: {ex}", sessionId, ex);
            }

            return clientMessage;
        }

        private ClientMessage? ProcessData(byte[] data, int bytes)
        {
            var position = 0;

            while (position < bytes)
            {
                if (currentPacketLength == -1 && bytes >= INT_SIZE)
                    currentPacketLength = data.ToInt32(ref position);

                if (!IsValidPacketLength(currentPacketLength))
                {
                    ResetState();
                    return default;
                }

                if (EnoughDataReceived(bytes - position))
                    return ProcessCompletePacket(data, ref position);
                else
                {
                    StoreIncompleteData(data, bytes, position);
                    return default;
                }
            }

            return default;
        }

        private bool EnoughDataReceived(int remainingBytes)
            => currentPacketLength <= remainingBytes + bufferPos;

        private static bool IsValidPacketLength(int length)
            => length is >= 2 and <= 417792;

        private ClientMessage? ProcessCompletePacket(byte[] data, ref int position)
        {
            if (bufferPos > 0)
            {
                Buffer.BlockCopy(data, position, bufferedData, bufferPos, currentPacketLength - bufferPos);
                position += currentPacketLength - bufferPos;
            }

            var packetData = bufferPos > 0 ? bufferedData : data;
            var packetStart = bufferPos > 0 ? 0 : position;
            var header = packetData.ToInt16(ref packetStart);

            var message = ClientMessageFactory.GetClientMessage(packetData, packetStart, header);

            position += currentPacketLength;

            ResetState();

            return message;
        }

        private void StoreIncompleteData(byte[] data, int bytes, int position)
        {
            int lengthToCopy = bytes - position;

            if (bufferPos + lengthToCopy > bufferedData.Length)
                throw new OverflowException("Buffer overflow detected in StoreIncompleteData");

            Buffer.BlockCopy(data, position, bufferedData, bufferPos, lengthToCopy);

            bufferPos += lengthToCopy;
        }

        private void ResetState()
        {
            bufferPos = 0;
            currentPacketLength = -1;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}