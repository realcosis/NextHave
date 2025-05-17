using NextHave.Utils;
using NextHave.Clients;
using NextHave.Messages.Output;
using System.Collections.Concurrent;

namespace NextHave.Messages.Input
{
    public class InputHandler
    {
        public delegate void Handled();

        public readonly static ConcurrentDictionary<short, Func<InputHandler, Task>> handlers = [];

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

        internal static async Task SSOLogin(InputHandler handler)
            => await handler.SSOLogin();

        public async Task Handle(ClientMessage message, short header)
        {
            if (header < 0 || header >= 4095)
                return;

            RegisterPacketLibary();
            var now = DateTime.Now;
            Message = message;

            if (handlers.TryGetValue(header, out var handler) && handler != default)
                await handler(this);
        }

        public async Task SSOLogin()
        {
            if (Client != default)
            {
                var authTicket = Message?.ReadString();
                if (string.IsNullOrWhiteSpace(authTicket))
                    return;

                var serverMessage = ServerMessageFactory.GetServerMessage(OutputCode.AuthenticationOKComposer);
                await Client.Send(Client.SessionId!.GetSessionChannel(), serverMessage.Bytes());

                PacketHandled?.Invoke();
            }
        }

        public void ClearMessage()
            => Message = default;
    }
}