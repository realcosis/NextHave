using NextHave.Clients;

namespace NextHave.Messages
{
    public interface IParser
    {
        public IMessageEvent Parse(ClientMessage packet);

        public Task HandleAsync(Client client, ClientMessage message, IPacketsService packetsService);
    }
}