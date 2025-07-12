using System.Buffers;
using NextHave.BL.Utils;
using NextHave.BL.Messages;
using Dolphin.Core.Injection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.PacketParsers
{
    [Service(ServiceLifetime.Scoped)]
    class GamePacketParser : IPacketParser
    {
        readonly ILogger<GamePacketParser> _logger;
        readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;

        const int INT_SIZE = 4;
        const int SHORT_SIZE = 2;
        const int MIN_PACKET_SIZE = 2;
        const int MAX_PACKET_SIZE = 417792;
        const int INITIAL_BUFFER_SIZE = 4096;

        int _currentPacketLength = -1;
        int _bufferPosition = 0;

        byte[] _rentedBuffer = [];

        private Memory<byte> _activeBuffer;

        public GamePacketParser(ILogger<GamePacketParser> logger)
        {
            _logger = logger;
            RentNewBuffer(INITIAL_BUFFER_SIZE);
        }

        public ClientMessage? HandlePacket(byte[] data, int bytes, string sessionId)
        {
            if (data == default || bytes <= 0)
                return null;

            try
            {
                return ProcessData(data.AsMemory(0, bytes));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing packet data for session {SessionId}", sessionId);
                ResetState();
                return null;
            }
        }

        public void Dispose()
        {
            if (_rentedBuffer != default)
            {
                _arrayPool.Return(_rentedBuffer, clearArray: true);
                _rentedBuffer = [];
                _activeBuffer = Memory<byte>.Empty;
            }

            GC.SuppressFinalize(this);
        }

        ClientMessage? ProcessData(ReadOnlyMemory<byte> data)
        {
            var dataSpan = data.Span;
            var position = 0;

            while (position < dataSpan.Length)
            {
                if (_currentPacketLength == -1)
                {
                    var remainingBytes = dataSpan.Length - position;

                    if (_bufferPosition > 0 || remainingBytes < INT_SIZE)
                    {
                        var bytesNeeded = INT_SIZE - _bufferPosition;
                        var bytesToCopy = Math.Min(bytesNeeded, remainingBytes);

                        StoreInBuffer(dataSpan.Slice(position, bytesToCopy));
                        position += bytesToCopy;

                        if (_bufferPosition >= INT_SIZE)
                        {
                            var bufferSpan = _activeBuffer.Span;
                            _currentPacketLength = bufferSpan[..INT_SIZE].ToInt32(position);

                            if (!IsValidPacketLength(_currentPacketLength))
                            {
                                _logger.LogWarning("Invalid packet length: {Length}", _currentPacketLength);
                                ResetState();
                                return null;
                            }

                            _bufferPosition = INT_SIZE;
                        }
                        continue;
                    }
                    else
                    {
                        _currentPacketLength = dataSpan.Slice(position, INT_SIZE).ToInt32(position);
                        position += INT_SIZE;

                        if (!IsValidPacketLength(_currentPacketLength))
                        {
                            _logger.LogWarning("Invalid packet length: {Length}", _currentPacketLength);
                            ResetState();
                            return null;
                        }
                    }
                }

                var remainingPacketBytes = _currentPacketLength - _bufferPosition;
                var availableBytes = dataSpan.Length - position;

                if (availableBytes >= remainingPacketBytes)
                    return CompletePacket(dataSpan, ref position, remainingPacketBytes);
                else
                {
                    StoreInBuffer(dataSpan.Slice(position, availableBytes));
                    position += availableBytes;
                }
            }

            return default;
        }

        ClientMessage? CompletePacket(ReadOnlySpan<byte> dataSpan, ref int position, int remainingBytes)
        {
            byte[] completePacket;
            if (_bufferPosition > 0)
            {
                completePacket = new byte[_currentPacketLength];
                var bufferSpan = _activeBuffer.Span;

                bufferSpan[.._bufferPosition].CopyTo(completePacket);

                dataSpan.Slice(position, remainingBytes).CopyTo(completePacket.AsSpan(_bufferPosition));
                position += remainingBytes;
            }
            else
            {
                completePacket = new byte[_currentPacketLength];
                dataSpan.Slice(INT_SIZE, _currentPacketLength).CopyTo(completePacket);
                position += remainingBytes;
            }

            var header = completePacket.ToInt16(0);

            var message = ClientMessageFactory.GetClientMessage(completePacket, SHORT_SIZE, header);

            ResetState();
            return message;
        }

        void StoreInBuffer(ReadOnlySpan<byte> data)
        {
            if (data.Length == 0)
                return;

            if (_bufferPosition + data.Length > _activeBuffer.Length)
                ExpandBuffer(_bufferPosition + data.Length);

            data.CopyTo(_activeBuffer.Span[_bufferPosition..]);
            _bufferPosition += data.Length;
        }

        void ExpandBuffer(int requiredSize)
        {
            var newSize = Math.Max(_activeBuffer.Length * 2, requiredSize);

            newSize = Math.Min(newSize, MAX_PACKET_SIZE);

            _logger.LogDebug("Expanding buffer from {OldSize} to {NewSize}", _activeBuffer.Length, newSize);

            var newBuffer = _arrayPool.Rent(newSize);

            try
            {
                if (_bufferPosition > 0)
                    _activeBuffer[.._bufferPosition].CopyTo(newBuffer);

                if (_rentedBuffer != default)
                    _arrayPool.Return(_rentedBuffer, clearArray: true);

                _rentedBuffer = newBuffer;
                _activeBuffer = _rentedBuffer.AsMemory(0, newSize);
            }
            catch
            {
                _arrayPool.Return(newBuffer, clearArray: true);
                throw;
            }
        }

        void RentNewBuffer(int size)
        {
            _rentedBuffer = _arrayPool.Rent(size);
            _activeBuffer = _rentedBuffer.AsMemory(0, size);
            _bufferPosition = 0;
        }

        void ResetState()
        {
            _currentPacketLength = -1;
            _bufferPosition = 0;

            if (_activeBuffer.Length > INITIAL_BUFFER_SIZE * 4)
            {
                if (_rentedBuffer != null)
                {
                    _arrayPool.Return(_rentedBuffer, clearArray: true);
                }
                RentNewBuffer(INITIAL_BUFFER_SIZE);
            }
        }

        static bool IsValidPacketLength(int length)
            => length is >= MIN_PACKET_SIZE and <= MAX_PACKET_SIZE;
    }
}