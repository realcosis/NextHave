using NextHave.BL.Utils;
using System.Text;

namespace NextHave.BL.Messages
{
    public class ClientMessage(byte[] body, int position, string sessionId) : IDisposable
    {
        private readonly static Encoding Encoding = Encoding.UTF8;

        public int RemainingLength
            => body.Length - position;

        public string SessionId
            => sessionId;

        public int Position
            => position;

        public byte[] Content
            => body;

        public int ReadShort()
            => body.ToInt16(ref position);

        public string ReadString()
        {
            var length = ReadShort();
            if (length == 0 || position + length > body.Length)
                return string.Empty;
            var value = Encoding.GetString(body, position, length);
            position += length;
            return value;
        }

        public int ReadInt()
            => body.ToInt32(ref position);

        public bool ReadBool()
            => body[position++] == 1;

        public byte[] ReadBytes(int bytes)
        {
            if (bytes > RemainingLength)
                bytes = RemainingLength;
            var array = new byte[bytes];
            for (int i = 0; i < bytes; i++)
                array[i] = body[position++];
            return array;
        }

        public void Dispose()
        {
            body = [];
            position = default;
            sessionId = string.Empty;
            GC.SuppressFinalize(this);
        }
    }
}