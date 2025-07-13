namespace NextHave.Gateway.Models
{
    class Connection
    {
        public int ActiveConnections;

        public DateTime LastActivity = DateTime.UtcNow;
    }
}