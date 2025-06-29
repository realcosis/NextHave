using Microsoft.AspNetCore.SignalR;
using NextHave.BL.Clients;
using NextHave.BL.Events.Rooms.Users;
using NextHave.BL.PacketParsers;

namespace NextHave.Nitro.Sockets
{
    public class SocketHub(IServiceScopeFactory serviceScopeFactory) : Hub
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
                if (client.User?.CurrentRoomInstance != default)
                {
                    await client.User.CurrentRoomInstance.EventsService.DispatchAsync(new UserRoomExitEvent
                    {
                        UserId = client.User.Id,
                        RoomId = client.User.CurrentRoomId!.Value,
                    });
                    client.User.CurrentRoomInstance = default;
                    client.User.CurrentRoomId = default;
                }
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