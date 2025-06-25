using Dolphin.Core.Events;
using Dolphin.Core.Injection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Clients;
using NextHave.BL.Events.Users;
using NextHave.BL.Messages.Input.Handshake;
using NextHave.BL.Messages.Output;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Packets;
using NextHave.BL.Services.Users;
using NextHave.BL.Utils;

namespace NextHave.BL.Messages.Input.Handlers
{
    [Service(ServiceLifetime.Singleton)]
    class MessageHandler(IPacketsService packetsService, IServiceScopeFactory serviceScopeFactory) : IMessageHandler, IStartableService
    {
        public async Task OnSSOTicket(SSOTicketMessage message, Client client)
        {
            using var scope = serviceScopeFactory.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;
            var usersService = serviceProvider.GetRequiredService<IUsersService>();
            var user = await usersService.LoadHabbo(message.SSO!, message.ElapsedMilliseconds);

            if (user == default)
                return;

            var eventsService = serviceProvider.GetRequiredService<IEventsService>();
            user.Client = client;
            client.User = user;
            await SendSSOTicketResponse(client, user);
            await eventsService.DispatchAsync<UserConnected>(new()
            {
                UserId = user.Id
            });
        }

        public async Task OnInfoRetrieve(InfoRetrieveMessage message, Client client)
        {
            if (client.User == default)
                return;

            await SendInfoRetrieveResponse(client, client.User!);
        }

        public async Task StartAsync()
        {

            packetsService.Subscribe<SSOTicketMessage>(this, OnSSOTicket);

            packetsService.Subscribe<InfoRetrieveMessage>(this, OnInfoRetrieve);

            await Task.CompletedTask;
        }

        #region private methods

        static async Task SendSSOTicketResponse(Client client, User user)
        {
            await using var authenticationOKComposer = ServerMessageFactory.GetServerMessage(OutputCode.AuthenticationOKMessageComposer);
            await client.Send(authenticationOKComposer.Bytes());

            await using var availabilityStatusMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.AvailabilityStatusMessageComposer);
            availabilityStatusMessageComposer.AddBoolean(true);
            availabilityStatusMessageComposer.AddBoolean(false);
            availabilityStatusMessageComposer.AddBoolean(true);
            await client.Send(availabilityStatusMessageComposer.Bytes());

            await using var userRightsMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.UserRightsMessageComposer);
            userRightsMessageComposer.AddInt32(2);
            userRightsMessageComposer.AddInt32(user.Rank);
            userRightsMessageComposer.AddBoolean(false);
            await client.Send(userRightsMessageComposer.Bytes());
        }

        static async Task SendInfoRetrieveResponse(Client client, User user)
        {
            await using var userObjectMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.UserObjectMessageComposer);

            userObjectMessageComposer.AddInt32(user.Id);
            userObjectMessageComposer.AddString(user.Username!);
            userObjectMessageComposer.AddString(user.Look!);
            userObjectMessageComposer.AddString(user.Gender!.ToUpper());
            userObjectMessageComposer.AddString(user.Motto ?? string.Empty);
            userObjectMessageComposer.AddString(string.Empty);
            userObjectMessageComposer.AddBoolean(false);
            userObjectMessageComposer.AddInt32(0);
            userObjectMessageComposer.AddInt32(0);
            userObjectMessageComposer.AddInt32(0);
            userObjectMessageComposer.AddBoolean(false);
            userObjectMessageComposer.AddString(user.LastOnline!.Value.GetDifference().ToString());
            userObjectMessageComposer.AddBoolean(false);
            userObjectMessageComposer.AddBoolean(false);

            await client.Send(userObjectMessageComposer.Bytes());
        }

        #endregion
    }
}