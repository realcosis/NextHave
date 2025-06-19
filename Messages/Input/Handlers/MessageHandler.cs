using Dolphin.Core.Injection;
using NextHave.Clients;
using NextHave.Messages.Input.Handshake;
using NextHave.Messages.Output;
using NextHave.Models.Users;
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
            await SendSSOTicketResponse(client, user);
        }

        public async Task StartAsync()
        {
            packetsService.Subscribe<SSOTicketMessage>(this, OnSSOTicket);
            await Task.CompletedTask;
        }

        #region private methods

        static async Task SendSSOTicketResponse(Client client, User user)
        {
            await using var authenticationOKComposer = ServerMessageFactory.GetServerMessage(OutputCode.AuthenticationOKComposer);
            await client.Send(client.SessionId!.GetSessionChannel(), authenticationOKComposer.Bytes());

            await using var availabilityStatusMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.AvailabilityStatusMessageComposer);
            availabilityStatusMessageComposer.AddBoolean(true);
            availabilityStatusMessageComposer.AddBoolean(false);
            availabilityStatusMessageComposer.AddBoolean(true);
            await client.Send(client.SessionId!.GetSessionChannel(), availabilityStatusMessageComposer.Bytes());

            await using var userRightsMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.UserRightsMessageComposer);
            userRightsMessageComposer.AddInt32(2);
            userRightsMessageComposer.AddInt32(user.Rank);
            userRightsMessageComposer.AddBoolean(false);
            await client.Send(client.SessionId!.GetSessionChannel(), userRightsMessageComposer.Bytes());


        }

        #endregion
    }
}