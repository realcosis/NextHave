using NextHave.Gateway.Models;
using System.Collections.Concurrent;

namespace NextHave.Gateway.Services
{
    class ConnectionThrottler : IConnectionThrottler
    {
        IConnectionThrottler Instance => this;

        readonly ConcurrentDictionary<string, List<string>> keys = [];

        readonly ConcurrentDictionary<string, ConnectionContext> connections = [];

        readonly int maxConnectionsPerIp = 20;

        readonly int maxRefreshForSession = 100;

        readonly TimeSpan connectionTimeout = TimeSpan.FromMinutes(10);

        bool IConnectionThrottler.ShouldAllowConnection(string ip, string sessionId)
        {
            var connectionkey = $"{ip}_{sessionId}";

            if (!connections.TryGetValue(connectionkey, out var connection))
                return true;

            return connection.ActiveConnections < maxConnectionsPerIp && connection.RefreshCount < maxRefreshForSession;
        }

        void IConnectionThrottler.RegisterConnection(string ip, string sessionId)
        {
            var connectionkey = $"{ip}_{sessionId}";

            if (connections.TryGetValue(connectionkey, out var connection))
            {
                connection.RefreshCount++;
                connection.LastActivity = DateTime.Now;
                return;
            }

            connection = new ConnectionContext
            {
                Key = connectionkey,
                ActiveConnections = keys.TryGetValue(ip, out var connectionIds) ? connectionIds.Count : 1,
                SessionId = sessionId,
                LastActivity = DateTime.Now,
                RefreshCount = 0
            };
            UpdateKeys(ip, sessionId);
            connections.TryAdd(connectionkey, connection);
        }

        void IConnectionThrottler.ReleaseConnection(string ip, ConnectionContext connection)
        {
            if (connection.ActiveConnections > 0)
                connection.ActiveConnections--;
            if (connection.ActiveConnections <= 0)
            {
                connection.ActiveConnections = 0;
                if (keys.TryGetValue(ip, out var connectionIds))
                    connectionIds.Remove(connection.SessionId!);
                connections.TryRemove(connection.Key!, out var _);
            }
        }

        void IConnectionThrottler.CleanupConnections()
        {
            var cutoff = DateTime.Now - connectionTimeout;
            var staleIps = connections.Where(kvp => kvp.Value.LastActivity < cutoff).ToList();
            foreach (var ip in staleIps)
                Instance.ReleaseConnection(ip.Key, ip.Value);
        }

        #region private methods

        void UpdateKeys(string ip, string connectionId)
        {
            if (keys.TryGetValue(ip, out var connections))
                connections.Add(connectionId);
            else
                keys.TryAdd(ip, [connectionId]);
        }

        #endregion
    }
}