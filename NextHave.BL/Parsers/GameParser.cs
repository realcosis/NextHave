using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Messages;
using NextHave.BL.Utils;

namespace NextHave.BL.Parsers
{
    [Service(ServiceLifetime.Scoped)]
    public class GameParser : IPacketParser
    {
        static readonly int INT_SIZE = 4;

        int currentPacketLength;

        int bufferPos;

        readonly byte[] bufferedData;

        public GameParser()
        {
            ResetState();
            bufferedData = new byte[4096];
        }

        public void HandlePacketData(byte[] data, int bytes, string sessionId)
        {
            try
            {
                ProcessData(data, bytes, sessionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public ClientMessage? ManagePacket(byte[] data, int bytes, string sessionId)
        {
            var clientMessage = default(ClientMessage?);

            try
            {
                clientMessage = ProcessData(data, bytes, sessionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return clientMessage;
        }

        private ClientMessage? ProcessData(byte[] data, int bytes, string sessionId)
        {
            int position = 0;
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
                    return ProcessCompletePacket(data, ref position, sessionId);
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

        private ClientMessage? ProcessCompletePacket(byte[] data, ref int position, string sessionId)
        {
            if (bufferPos > 0)
            {
                Buffer.BlockCopy(data, position, bufferedData, bufferPos, currentPacketLength - bufferPos);
                position += currentPacketLength - bufferPos;
            }

            var packetData = bufferPos > 0 ? bufferedData : data;
            var packetStart = bufferPos > 0 ? 0 : position;
            var header = packetData.ToInt16(ref packetStart);

            var message = ClientMessageFactory.GetClientMessage(packetData, packetStart, sessionId, header);

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