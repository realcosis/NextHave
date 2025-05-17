using System.Text;
using NextHave.Utils;

namespace NextHave.Messages
{
    public class ClientMessage(byte[] body, int position, string sessionId) : IDisposable
    {
        private readonly static Encoding Encoding = Encoding.UTF8;

        public int RemainingLength
            => body.Length - position;

        public string SessionId
            => sessionId;

        public int ReadShort()
            => body.ToInt16(ref position);

        public string ReadString()
        {
            int num = ReadShort();
            if (num == 0 || position + num > body.Length)
                return string.Empty;
            var @string = Encoding.GetString(body, position, num);
            position += num;
            return @string;
        }

        public int ReadInt()
            => body.ToInt32(ref position);

        public uint ReadUInt()
            => (uint)ReadInt();

        public bool ReadBool()
            => body[position++] == 1;

        public byte[] ReadBytes(int bytes)
        {
            if (bytes > RemainingLength)
                bytes = RemainingLength;
            var array = new byte[bytes];
            for (int i = 0; i < bytes; i++)
            {
                array[i] = body[position++];
            }
            return array;
        }

        public override string ToString()
            => $"BODY: {Encoding.GetString(body)}";

        public void Dispose()
        {
            // ClientMessageFactory.Enqueue(this); TODO: Packet Queue
            GC.SuppressFinalize(this);
        }
    }
}