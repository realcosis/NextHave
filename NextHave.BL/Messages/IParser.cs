using NextHave.BL.Clients;
using NextHave.BL.Services.Packets;

namespace NextHave.BL.Messages
{
    public interface IParser
    {
        public IMessageEvent Parse(ClientMessage packet);

        public Task HandleAsync(IClient client, ClientMessage message, IPacketsService packetsService);
    }
}