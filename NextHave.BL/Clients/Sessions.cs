using System.Collections.Concurrent;

namespace NextHave.BL.Clients
{
    public static class Sessions
    {
        public static readonly ConcurrentDictionary<string, Client> ConnectedClients = [];
    }
}