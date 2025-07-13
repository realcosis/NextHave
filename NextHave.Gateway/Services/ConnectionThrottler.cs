using NextHave.Gateway.Models;
using System.Collections.Concurrent;

namespace NextHave.Gateway.Services
{
    public class ConnectionThrottler : IConnectionThrottler
    {
        IConnectionThrottler Instance => this;

        readonly ConcurrentDictionary<string, Connection> connections = [];

        readonly int maxConnectionsPerIp = 10;

        readonly TimeSpan connectionTimeout = TimeSpan.FromMinutes(20);

        bool IConnectionThrottler.ShouldAllowConnection(string ip)
        {
            var info = connections.GetOrAdd(ip, _ => new Connection());
            return info.ActiveConnections < maxConnectionsPerIp;
        }

        void IConnectionThrottler.RegisterConnection(string ip)
        {
            var info = connections.GetOrAdd(ip, _ => new Connection());
            Interlocked.Increment(ref info.ActiveConnections);
            info.LastActivity = DateTime.UtcNow;
        }

        void IConnectionThrottler.ReleaseConnection(string ip)
        {
            if (connections.TryGetValue(ip, out var info))
            {
                if (info.ActiveConnections <= 0)
                    info.ActiveConnections = 1;
                if (info.ActiveConnections > 0)
                    Interlocked.Decrement(ref info.ActiveConnections);
            }
        }

        void IConnectionThrottler.CleanupConnections()
        {
            var cutoff = DateTime.UtcNow - connectionTimeout;
            var staleIps = connections.Where(kvp => kvp.Value.LastActivity < cutoff).Select(kvp => kvp.Key).ToList();
            foreach (var ip in staleIps)
                Instance.ReleaseConnection(ip);
        }
    }
}