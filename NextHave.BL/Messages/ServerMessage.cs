using System.Text;

namespace NextHave.BL.Messages
{
    public class ServerMessage(short header) : IAsyncDisposable
    {
        byte[] data = [];

        int position;

        int currentLength;

        int currentMaxSize;

        public void Init()
        {
            position = 4;
            currentLength = 0;
            currentMaxSize = 4;
            data = new byte[currentMaxSize];
            AddInt16(header);
        }

        public void AddInt16(short value)
        {
            EnsureCapacity(2);
            data[position++] = (byte)(0xFF & (value >> 8));
            data[position++] = (byte)(0xFF & value);
            currentLength += 2;
        }

        public void AddInt32(int value)
        {
            EnsureCapacity(4);
            data[position++] = (byte)(0xFF & (value >> 24));
            data[position++] = (byte)(0xFF & (value >> 16));
            data[position++] = (byte)(0xFF & (value >> 8));
            data[position++] = (byte)(0xFF & value);
            currentLength += 4;
        }

        public void AddChar(char value)
        {
            EnsureCapacity(1);
            AddInt16(1);
            data[position++] = (byte)(0xFF & value);
            currentLength++;
        }

        public void AddBoolean(bool value)
            => AddByte(value ? (byte)1 : (byte)0);

        public void AddString(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var length = (short)bytes.Length;
            AddInt16(length);
            AddBytes(bytes);
        }

        public void AddByte(byte value)
        {
            EnsureCapacity(1);
            data[position++] = value;
            currentLength++;
        }

        public void AddBytes(byte[] value)
        {
            EnsureCapacity(value.Length);
            for (int i = 0; i < value.Length; i++)
                data[position++] = value[i];
            currentLength += value.Length;
        }

        public byte[] Bytes()
        {
            data[0] = (byte)(0xFF & (currentLength >> 24));
            data[1] = (byte)(0xFF & (currentLength >> 16));
            data[2] = (byte)(0xFF & (currentLength >> 8));
            data[3] = (byte)(0xFF & currentLength);
            return data;
        }

        private void EnsureCapacity(int length)
        {
            if (position + length < 417792)
                IncreaseSize(length);
        }

        void IncreaseSize(int bytes)
        {
            currentMaxSize = bytes;
            var newData = new byte[bytes];
            data = [.. data, .. newData];
        }

        public ValueTask DisposeAsync()
        {
            data = [];
            Init();
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }
}