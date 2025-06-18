using Dolphin.Core.Injection;
using NextHave.Clients;
using NextHave.Messages.Input.Handshake;
using NextHave.Messages.Output;
using NextHave.Services.Users;
using NextHave.Utils;

namespace NextHave.Messages.Input.Handlers
{
    [Service(ServiceLifetime.Singleton)]
    class MessageHandler(IPacketsService packetsService, IServiceScopeFactory serviceScopeFactory) : IMessageHandler, IStartableService
    {
        public async Task OnSSOTicket(SSOTicketMessage message, Client client)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();
            var user = await usersService.LoadHabbo(message.SSO!, message.ElapsedMilliseconds);

            if (user == default)
                return;

            user.Client = client;
            await using var serverMessage = ServerMessageFactory.GetServerMessage(OutputCode.AuthenticationOKComposer);
            await client.Send(client.SessionId!.GetSessionChannel(), serverMessage.Bytes());
        }

        public async Task StartAsync()
        {
            packetsService.Subscribe<SSOTicketMessage>(this, OnSSOTicket);
            await Task.CompletedTask;
        }
    }
}