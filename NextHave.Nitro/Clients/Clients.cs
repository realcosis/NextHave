using System.Collections.Concurrent;

namespace NextHave.Nitro.Clients
{
    public static class Clients
    {
        public static readonly ConcurrentDictionary<string, Client> ConnectedClients = [];
    }
}