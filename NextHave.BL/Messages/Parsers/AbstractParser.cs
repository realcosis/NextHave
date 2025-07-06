using NextHave.BL.Clients;
using NextHave.BL.Services.Packets;
using System.Collections.Concurrent;

namespace NextHave.BL.Messages.Parsers
{
    public abstract class AbstractParser<T> : IParser where T : IInput
    {
        public async virtual Task HandleAsync(Client client, ClientMessage message, IPacketsService packetsService)
        {
            var messageEvent = (T)Parse(message);
            await packetsService.Publish(messageEvent, client);
        }

        abstract public IInput Parse(ClientMessage packet);
    }
}