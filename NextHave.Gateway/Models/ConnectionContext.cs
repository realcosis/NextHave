namespace NextHave.Gateway.Models
{
    public class ConnectionContext
    {
        public string? Key { get; set; }

        public string? SessionId { get; set; }

        public int ActiveConnections { get; set; }

        public int RefreshCount { get; set; }

        public DateTime LastActivity { get; set; } = DateTime.Now;
    }
}