using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Messages;
using NextHave.BL.Messages.Input;
using NextHave.BL.Messages.Parsers;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Parsers
{
    [Service(ServiceLifetime.Singleton)]
    class ParsersService : IParsersService, IStartableService
    {
        ConcurrentDictionary<short, IParser> parsers = [];

        async Task IStartableService.StartAsync()
        {
            RegisterDefaultParsers();
            await Task.CompletedTask;
        }

        bool IParsersService.TryGetParser(short header, out IParser? parser)
        {
            if (parsers.TryGetValue(header, out var p))
            {
                parser = p;
                return true;
            }
            parser = default;
            return false;
        }

        void IParsersService.UpsertParser(short header, IParser parser)
        {
            if (parsers.ContainsKey(header))
                parsers[header] = parser;
            else
                parsers.TryAdd(header, parser);
        }

        public void RegisterDefaultParsers()
        {
            parsers.TryAdd(InputCode.SSOTicketMessageEvent, new SSOTicketMessageParser());

            parsers.TryAdd(InputCode.InfoRetrieveMessageEvent, new InfoRetrieveParser());
        }
    }
}