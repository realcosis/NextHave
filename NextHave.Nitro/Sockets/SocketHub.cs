using Dolphin.Core.Events;
using Microsoft.AspNetCore.SignalR;
using NextHave.BL.Clients;
using NextHave.BL.Events.Users;
using NextHave.BL.PacketParsers;

namespace NextHave.Nitro.Sockets
{
    public class SocketHub(IServiceScopeFactory serviceScopeFactory, IEventsService eventsService) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (!Sessions.ConnectedClients.TryGetValue(Context.ConnectionId, out var client))
            {
                client = new Client();
                client.Init(Context.ConnectionId, Clients.Client(Context.ConnectionId));
            }
            Sessions.ConnectedClients.TryAdd(Context.ConnectionId, client);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Sessions.ConnectedClients.TryGetValue(Context.ConnectionId, out var client))
            {
                await eventsService.DispatchAsync<UserDisconnected>(new()
                {
                    UserId = client.User?.Id
                });
                Sessions.ConnectedClients.TryRemove(Context.ConnectionId, out _);
            }

            await base.OnDisconnectedAsync(exception);
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
        }
    }
}