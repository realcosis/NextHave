using Microsoft.AspNetCore.SignalR;
using NextHave.BL.Clients;
using NextHave.BL.PacketParsers;

namespace NextHave.Nitro.Sockets
{
    public class SocketHub(IServiceScopeFactory serviceScopeFactory) : Hub
    {
        public override Task OnConnectedAsync()
        {
            if (!Sessions.ConnectedClients.TryGetValue(Context.ConnectionId, out var client))
            {
                client = new Client();
                client.Init(Context.ConnectionId, Clients.Client(Context.ConnectionId));
            }
            Sessions.ConnectedClients.TryAdd(Context.ConnectionId, client);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (Sessions.ConnectedClients.TryGetValue(Context.ConnectionId, out var _))
                Sessions.ConnectedClients.TryRemove(Context.ConnectionId, out _);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task ReceiveBinary(List<byte> input)
        {
            var buffer = input.ToArray();
            if (Sessions.ConnectedClients.TryGetValue(Context.ConnectionId, out var client))
            {
                using var scope = serviceScopeFactory.CreateAsyncScope();
                var gamePacketParser = scope.ServiceProvider.GetRequiredService<IPacketParser>();
                using var message = gamePacketParser.HandlePacket(buffer, buffer.Length, Context.ConnectionId);
                if (message != default)
                    await client.Handler!.Handle(message, serviceScopeFactory, message.Header);
            }
            await Task.CompletedTask;
        }
    }
}