namespace NextHave.BL.Clients
{
    public interface IClient
    {
        string? SessionId { get; }

        List<string> Channels { get; }

        Task Send(string channel, byte[] output);
    }
}