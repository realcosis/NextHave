using NextHave.Clients;

namespace NextHave.Messages.Parsers
{
    public abstract class AbstractParser<T> : IParser
        where T : IMessageEvent
    {
        public async virtual Task HandleAsync(Client client, ClientMessage message, IPacketsService packetsService)
        {
            T messageEvent = (T)Parse(message);
            await packetsService.Publish(messageEvent, client);
        }

        abstract public IMessageEvent Parse(ClientMessage packet);
    }
}
