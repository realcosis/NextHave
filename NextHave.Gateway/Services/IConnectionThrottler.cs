using NextHave.Gateway.Models;

namespace NextHave.Gateway.Services
{
    public interface IConnectionThrottler
    {
        bool ShouldAllowConnection(string ip, string sessionId);

        void RegisterConnection(string ip, string sessionId);

        void ReleaseConnection(string ip, ConnectionContext connection);

        void CleanupConnections();
    }
}