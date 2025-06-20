using NextHave.BL.Clients;
using NextHave.BL.Services.Packets;

namespace NextHave.BL.Messages
{
    public interface IParser
    {
        public IInput Parse(ClientMessage packet);

        public Task HandleAsync(Client client, ClientMessage message, IPacketsService packetsService);
    }
}