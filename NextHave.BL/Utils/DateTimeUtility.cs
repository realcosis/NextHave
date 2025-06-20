namespace NextHave.BL.Utils
{
    public static class DateTimeUtility
    {
        public static int GetDifference(this DateTime dateTime)
            => (int)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
    }
}