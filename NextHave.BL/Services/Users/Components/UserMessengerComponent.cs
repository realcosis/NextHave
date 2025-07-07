using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Clients;
using NextHave.BL.Events.Users.Messenger;
using NextHave.BL.Mappers;
using NextHave.BL.Messages.Input.Messenger;
using NextHave.BL.Messages.Output.Messenger;
using NextHave.BL.Models.Users.Messenger;
using NextHave.BL.Services.Packets;
using NextHave.BL.Services.Users.Instances;
using NextHave.DAL.MySQL;

namespace NextHave.BL.Services.Users.Components
{
    [Service(ServiceLifetime.Scoped)]
    class UserMessengerComponent(IServiceScopeFactory serviceScopeFactory, IPacketsService packetsService) : IUserComponent
    {
        IUserInstance? _userInstance;

        List<MessengerBuddy> Friends { get; set; } = [];

        List<MessengerRequest> Requests { get; set; } = [];

        async Task IUserComponent.Dispose()
        {
            if (_userInstance == default)
                return;

            await _userInstance.EventsService.UnsubscribeAsync<FriendStatusChangedEvent>(_userInstance, OnFriendStatusChanged);

            Friends.Clear();
            Requests.Clear();
            _userInstance = default;
        }

        async Task IUserComponent.Init(IUserInstance userInstance)
        {
            if (userInstance?.User == default)
                return;

            _userInstance = userInstance;

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            await scope.DisposeAsync();

            var senderFriendships = (await mysqlDbContext
                                                   .MessengerFriendships
                                                       .AsNoTracking()
                                                       .Include(mf => mf.ReceiverUser)
                                                       .Where(mf => mf.Sender == userInstance.User.Id)
                                                       .ToListAsync()).Select(mf => mf.MapReceiver()).ToList();

            var receiveriendships = (await mysqlDbContext
                                                .MessengerFriendships.AsNoTracking()
                                                     .Include(mf => mf.SenderUser)
                                                     .Where(mf => mf.Receiver == userInstance.User.Id)
                                                     .ToListAsync()).Select(mf => mf.MapSender()).ToList();

            Requests = [.. (await mysqlDbContext
                                            .MessengerRequests
                                                .AsNoTracking()
                                                .Include(mf => mf.SenderUser)
                                                .Where(mf => mf.Receiver == userInstance.User.Id)
                                                .ToListAsync()).Select(mf => mf.MapSender())];

            Friends = [.. senderFriendships.Union(receiveriendships)];

            packetsService.Subscribe<MessengerInitMessage>(_userInstance, OnMessengerInitMessage);

            await _userInstance.EventsService.SubscribeAsync<FriendStatusChangedEvent>(_userInstance, OnFriendStatusChanged);
        }

        public async Task OnFriendUpdate(FriendUpdateEvent @event)
        {
            var friend = Friends.FirstOrDefault(f => f.UserId == @event.FriendId);
            if (friend != default)
            {
                friend.CurrentRoom = @event.RoomInstance;
                friend.Client = @event.Client;

                if (@event.Notification && friend.Client != default && friend.Client.UserInstance != default)
                    await friend.Client.Send(new FriendListUpdateMessageComposer(friend.Client.UserInstance.User, friend, default));
            }
        }


        public async Task OnFriendStatusChanged(FriendStatusChangedEvent @event)
        {
            if (_userInstance?.User == default)
                return;

            var friendIds = Friends.Select(f => f.UserId).ToList();

            var clients = Sessions.ConnectedClients.Values.Where(c => c.UserInstance?.User != default && friendIds.Contains(c.UserInstance.User.Id)).ToList();

            if (clients.Count <= 0)
                return;

            var cancellationSource = new CancellationTokenSource();
            await Parallel.ForEachAsync(clients, new ParallelOptions()
            {
                CancellationToken = cancellationSource.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            }, async (client, token) =>
            {
                if (client.UserInstance?.User == default)
                    return;
                
                await client.UserInstance.EventsService.DispatchAsync<FriendUpdateEvent>(new()
                {
                    FriendId = @event.UserId,
                    Notification = true,
                    Client = client,
                    RoomInstance = client.UserInstance.CurrentRoomInstance,
                    UserId = client.UserInstance.User.Id
                });

                await _userInstance.EventsService.DispatchAsync<FriendUpdateEvent>(new()
                {
                    FriendId = client.UserInstance.User.Id,
                    Notification = @event.Notification,
                    Client = client,
                    RoomInstance = _userInstance.CurrentRoomInstance,
                    UserId = _userInstance.User.Id
                });
            });
        }

        public async Task OnMessengerInitMessage(MessengerInitMessage message, Client client)
        {
            if (_userInstance?.User == default)
                return;

            var friends = Friends.Where(b => b.Online).ToList();

            await client.Send(new MessengerInitMessageComposer());

            var page = 0;
            var pages = (friends.Count - 1) / 500 + 1;

            if (friends.Count == 0)
                await client.Send(new BuddyListMessageComposer(friends, pages, page));
            else
            {
                foreach (var batch in friends.Chunk(500))
                {
                    await client.Send(new BuddyListMessageComposer([.. batch], pages, page));
                    page++;
                }
            }
        }
    }
}