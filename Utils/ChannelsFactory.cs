namespace NextHave.Utils
{
    public static class ChannelsFactory
    {
        public static string GetSessionChannel(this string sessionId)
            => $"NextHave_s{sessionId}";
    }
}