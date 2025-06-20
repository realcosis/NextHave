namespace NextHave.BL.Utils
{
    public static class ChannelsFactory
    {
        public static string GetSessionChannel(this string sessionId)
            => $"NextHave_s{sessionId}";
    }
}