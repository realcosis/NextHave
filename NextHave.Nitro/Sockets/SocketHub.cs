using Microsoft.AspNetCore.SignalR;
using NextHave.BL.Clients;
using NextHave.BL.Parsers;

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
                var scope = serviceScopeFactory.CreateScope();
                var gameParser = scope.ServiceProvider.GetRequiredService<IPacketParser>();
                gameParser.HandlePacketData(buffer, buffer.Length, Context.ConnectionId);
                gameParser.OnNewPacket += client.Handler!.Handle;
            }
            await Task.CompletedTask;
        }
    }
}