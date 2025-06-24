using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Clients;
using NextHave.BL.Messages.Parsers;
using NextHave.BL.Services.Packets;

namespace NextHave.BL.Messages.Input
{
    public class InputHandler
    {
        public Client? Client { get; set; }

        public InputHandler()
        {
            if (!AbstractParser<IInput>.Registered)
                AbstractParser<IInput>.RegisterDefaultParsers();
        }

        public async Task Handle(ClientMessage message, IServiceScopeFactory serviceScopeFactory, short header)
        {
            if (Client == default || header < 0 || header >= 4095)
                return;

            using var scope = serviceScopeFactory.CreateScope();
            var packetsService = scope.ServiceProvider.GetRequiredService<IPacketsService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<InputHandler>>();
            if (AbstractParser<IInput>.TryGetParser(header, out var parser))
                await parser!.HandleAsync(Client!, message, packetsService);
        }
    }
}