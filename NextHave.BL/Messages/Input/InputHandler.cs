using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Clients;
using NextHave.BL.Services.Packets;
using NextHave.BL.Services.Parsers;

namespace NextHave.BL.Messages.Input
{
    public class InputHandler
    {
        public Client? Client { get; set; }

        public async Task Handle(ClientMessage message, IServiceScopeFactory serviceScopeFactory, short header)
        {
            if (Client == default || header < 0 || header >= 4095)
                return;

            var packetsService = await serviceScopeFactory.GetRequiredService<IPacketsService>();
            var parsersService = await serviceScopeFactory.GetRequiredService<IParsersService>();

            if (parsersService.TryGetParser(header, out var parser))
                await parser!.HandleAsync(Client!, message, packetsService);
        }
    }
}