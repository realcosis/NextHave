using Dolphin.Core.Injection;
using NextHave.Clients;
using NextHave.Messages.Input.Handshake;
using NextHave.Messages.Output;
using NextHave.Services.Users;
using NextHave.Utils;

namespace NextHave.Messages.Input.Handlers
{
    [Service(ServiceLifetime.Singleton)]
    class MessageHandler : IMessageHandler
    {
        readonly IPacketsService _packetsService;
        readonly ILogger<IMessageHandler> _logger;
        readonly IServiceScopeFactory _serviceScopeFactory;

        public MessageHandler(IPacketsService packetsService, ILogger<IMessageHandler> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _packetsService = packetsService;
            _serviceScopeFactory = serviceScopeFactory;

            _packetsService.Subscribe<SSOTicketMessage>(this, OnSSOTicket);
        }

        public async Task OnSSOTicket(SSOTicketMessage message, Client client)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            //var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();
            //var user = await usersService.LoadHabbo(message.SSO!, message.ElapsedMilliseconds);

            //if (user == default)
            //    return;

            //user.Client = client;
            await using var serverMessage = ServerMessageFactory.GetServerMessage(OutputCode.AuthenticationOKComposer);
            await client.Send(client.SessionId!.GetSessionChannel(), serverMessage.Bytes());
        }
    }
}