namespace NextHave.BL.Utils
{
    public static class EncodingUtility
    {
        public static short ToInt16(this byte[] array, ref int position)
        {
            if (position + 2 > array.Length || position < 0)
                return 0;

            return (short)((array[position++] << 8) + array[position++]);
        }

        public static int ToInt32(this byte[] array, ref int position)
        {
            if (position + 4 > array.Length || position < 0)
                return 0;

            return (array[position++] << 24) + (array[position++] << 16) + (array[position++] << 8) + array[position++];
        }
    }
}