using NextHave.BL.Clients;
using NextHave.BL.Services.Packets;

namespace NextHave.BL.Messages.Parsers
{
    public abstract class AbstractParser<T> : IParser
        where T : IMessageEvent
    {
        public async virtual Task HandleAsync(IClient client, ClientMessage message, IPacketsService packetsService)
        {
            var messageEvent = Parse(message);
            await packetsService.Publish(messageEvent, client);
        }

        abstract public IMessageEvent Parse(ClientMessage packet);
    }
}