using NextHave.BL.Clients;
using NextHave.BL.Messages.Input;
using NextHave.BL.Services.Packets;
using System.Collections.Concurrent;

namespace NextHave.BL.Messages.Parsers
{
    public abstract class AbstractParser<T> : IParser where T : IInput
    {
        readonly static ConcurrentDictionary<short, IParser> parsers = [];

        public async virtual Task HandleAsync(Client client, ClientMessage message, IPacketsService packetsService)
        {
            var messageEvent = (T)Parse(message);
            await packetsService.Publish(messageEvent, client);
        }

        abstract public IInput Parse(ClientMessage packet);

        public static bool TryGetParser(short header, out IParser? parser)
        {
            if (parsers.TryGetValue(header, out var p))
            {
                parser = p;
                return true;
            }
            parser = default;
            return false;
        }

        public static void UpsertParser(short header, IParser parser)
        {
            if (parsers.ContainsKey(header))
                parsers[header] = parser;
            else
                parsers.TryAdd(header, parser);
        }

        public static bool Registered
            => !parsers.IsEmpty;

        public static void RegisterDefaultParsers()
        {
            parsers.TryAdd(InputCode.SSOTicketMessageEvent, new SSOTicketMessageParser());

            parsers.TryAdd(InputCode.InfoRetrieveMessageEvent, new InfoRetrieveParser());
        }
    }
}