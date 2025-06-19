using NextHave.Clients;
using NextHave.Messages.Parsers;

namespace NextHave.Messages.Input
{
    public class InputHandler
    {
        readonly Dictionary<short, IParser> Parsers = [];

        public Client? Client { get; set; }

        public InputHandler()
        {
            RegisterParsers();
        }

        public async Task Handle(ClientMessage message, IServiceScopeFactory serviceScopeFactory, short header)
        {
            if (header < 0 || header >= 4095)
                return;

            message.Handler = this;
            using var scope = serviceScopeFactory.CreateScope();
            var packetsService = scope.ServiceProvider.GetRequiredService<IPacketsService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<InputHandler>>();
            if (TryGetParser(header, out var parser))
                await parser!.HandleAsync(Client!, message, packetsService);
            else 
                logger.LogDebug("No matching parser found for message {header}:{msg}", header, message);
            message.Handler = default;
        }

        #region private methods

        void RegisterParsers()
        {
            Parsers.Add(InputCode.SSOTicketMessageEvent, new SSOTicketMessageParser());

            Parsers.Add(InputCode.InfoRetrieveMessageEvent, new InfoRetrieveParser());
        }

        bool TryGetParser(short header, out IParser? parser)
        {
            if (Parsers.TryGetValue(header, out var p))
            {
                parser = p;
                return true;
            }
            parser = default;
            return false;
        }

        #endregion
    }
}