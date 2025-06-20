using NextHave.BL.Models.Users;
using NextHave.BL.Messages.Input;
using Microsoft.AspNetCore.SignalR;

namespace NextHave.BL.Clients
{
    public class Client
    {
        public string? ConnectionId { get; private set; }

        public InputHandler? Handler { get; set; }

        public IClientProxy? ClientProxy { get; set; }

        public User? User { get; set; }

        public void Init(string connectionId, IClientProxy clientProxy)
        {
            ConnectionId = connectionId;
            ClientProxy = clientProxy;
            Handler ??= new()
            {
                Client = this
            };
        }

        public async Task Send(byte[] output)
        {
            if (ClientProxy != default)
                await ClientProxy.SendAsync("ReceiveBinary", output);
        }
    }
}