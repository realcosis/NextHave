using Dolphin.Core.Injection;
using NextHave.Messages;
using NextHave.Utils;

namespace NextHave.Parsers
{
    [Service(ServiceLifetime.Singleton)]
    public class GameParser : IParser
    {
        readonly IServiceScopeFactory _serviceScopeFactory;

        public delegate void HandlePacket(ClientMessage message, short header);

        public event Func<ClientMessage, IServiceScopeFactory, short, Task>? OnNewPacket;

        static readonly int INT_SIZE = 4;

        int currentPacketLength;

        int bufferPos;

        readonly byte[] bufferedData;

        public GameParser(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
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

        private void ProcessData(byte[] data, int bytes, string sessionId)
        {
            int position = 0;
            while (position < bytes)
            {
                if (currentPacketLength == -1 && bytes >= INT_SIZE)
                    currentPacketLength = data.ToInt32(ref position);

                if (!IsValidPacketLength(currentPacketLength))
                {
                    ResetState();
                    return;
                }

                if (EnoughDataReceived(bytes - position))
                    ProcessCompletePacket(data, ref position, sessionId);
                else
                {
                    StoreIncompleteData(data, bytes, position);
                    break;
                }
            }
        }

        private bool EnoughDataReceived(int remainingBytes)
            => currentPacketLength <= remainingBytes + bufferPos;

        private static bool IsValidPacketLength(int length)
            => length is >= 2 and <= 417792;

        private void ProcessCompletePacket(byte[] data, ref int position, string sessionId)
        {
            if (bufferPos > 0)
            {
                Buffer.BlockCopy(data, position, bufferedData, bufferPos, currentPacketLength - bufferPos);
                position += currentPacketLength - bufferPos;
            }

            var packetData = bufferPos > 0 ? bufferedData : data;
            var packetStart = bufferPos > 0 ? 0 : position;
            var header = packetData.ToInt16(ref packetStart);

            using var message = ClientMessageFactory.GetClientMessage(packetData, packetStart, sessionId);
            OnNewPacket?.Invoke(message, _serviceScopeFactory, header);

            position += currentPacketLength;

            ResetState();
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
            OnNewPacket = default;
            GC.SuppressFinalize(this);
        }
    }
}