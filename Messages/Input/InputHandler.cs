using Microsoft.Extensions.DependencyInjection;
using NextHave.Clients;
using NextHave.Messages.Output;
using NextHave.Services.Users;
using NextHave.Utils;
using System.Collections.Concurrent;

namespace NextHave.Messages.Input
{
    public class InputHandler
    {
        public delegate void Handled();

        public readonly static ConcurrentDictionary<short, Func<InputHandler, IServiceScopeFactory, Task>> handlers = [];

        public static bool InputLoaded { get; private set; } = false;

        public static ClientMessage? Message { get; private set; }

        public Client? Client { get; set; }

        public event Handled? PacketHandled;

        public static void RegisterPacketLibary()
        {
            if (!InputLoaded)
            {
                handlers.TryAdd(InputCode.SSOTicketMessageEvent, SSOLogin);
                InputLoaded = true;
            }
        }

        public void Callback()
            => PacketHandled?.Invoke();

        public static async Task SSOLogin(InputHandler handler, IServiceScopeFactory serviceScopeFactory)
            => await handler.SSOLogin(serviceScopeFactory);

        public async Task Handle(ClientMessage message, IServiceScopeFactory serviceScopeFactory, short header)
        {
            if (header < 0 || header >= 4095)
                return;

            RegisterPacketLibary();
            var now = DateTime.Now;
            Message = message;

            if (handlers.TryGetValue(header, out var handler) && handler != default)
                await handler(this, serviceScopeFactory);
        }

        public async Task SSOLogin(IServiceScopeFactory serviceScopeFactory)
        {
            if (Client != default)
            {
                var authTicket = Message?.ReadString();
                if (string.IsNullOrWhiteSpace(authTicket))
                    return;

                using var scope = serviceScopeFactory.CreateScope();
                var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();
                var user = await usersService.LoadHabbo(authTicket);

                if (user != default)
                {
                    await using var serverMessage = ServerMessageFactory.GetServerMessage(OutputCode.AuthenticationOKComposer);
                    await Client.Send(Client.SessionId!.GetSessionChannel(), serverMessage.Bytes());
                }
            }
            Callback();
        }
    }
}