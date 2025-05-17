using System.Collections.Concurrent;

namespace NextHave.Clients
{
    public static class Clients
    {
        public static readonly ConcurrentDictionary<string, Client> ConnectedClients = [];
    }
}