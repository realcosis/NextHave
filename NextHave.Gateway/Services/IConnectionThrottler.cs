namespace NextHave.Gateway.Services
{
    public interface IConnectionThrottler
    {
        bool ShouldAllowConnection(string ip);

        void RegisterConnection(string ip);

        void ReleaseConnection(string ip);

        void CleanupConnections();
    }
}