namespace NextHave.Messages
{
    public class ServerMessage(short header)
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
    }
}