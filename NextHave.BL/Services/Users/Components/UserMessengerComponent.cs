using Dolphin.Core.Backgrounds;
using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Clients;
using NextHave.BL.Events.Users.Friends;
using NextHave.BL.Events.Users.Messenger;
using NextHave.BL.Mappers;
using NextHave.BL.Messages.Output.Friends;
using NextHave.BL.Messages.Output.Messenger;
using NextHave.BL.Models.Users.Messenger;
using NextHave.BL.Services.Users.Instances;
using NextHave.BL.Tasks.Rooms.Chat;
using NextHave.DAL.MySQL;

namespace NextHave.BL.Services.Users.Components
{
    [Service(ServiceLifetime.Scoped)]
    class UserMessengerComponent(IServiceScopeFactory serviceScopeFactory) : IUserComponent
    {
        IUserInstance? _userInstance;

        List<MessengerBuddy> Friends { get; set; } = [];

        List<MessengerRequest> Requests { get; set; } = [];

        Dictionary<string, string> Emojis = [];

        async Task IUserComponent.Dispose()
        {
            if (_userInstance == default)
                return;

            await _userInstance.EventsService.UnsubscribeAsync<MessengerInitMessageEvent>(_userInstance, OnMessengerInitMessage);

            await _userInstance.EventsService.UnsubscribeAsync<FriendStatusChangedEvent>(_userInstance, OnFriendStatusChanged);

            await _userInstance.EventsService.UnsubscribeAsync<FriendUpdateEvent>(_userInstance, OnFriendUpdate);

            await _userInstance.EventsService.UnsubscribeAsync<SendMessageEvent>(_userInstance, OnSendMessage);

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

            Emojis = await mysqlDbContext.RoomEmojis.AsNoTracking().ToDictionaryAsync(k => k.Code!, v => v.Emoji!);

            await _userInstance.EventsService.SubscribeAsync<MessengerInitMessageEvent>(_userInstance, OnMessengerInitMessage);

            await _userInstance.EventsService.SubscribeAsync<FriendStatusChangedEvent>(_userInstance, OnFriendStatusChanged);

            await _userInstance.EventsService.SubscribeAsync<FriendUpdateEvent>(_userInstance, OnFriendUpdate);

            await _userInstance.EventsService.SubscribeAsync<SendMessageEvent>(_userInstance, OnSendMessage);
        }

        async Task OnSendMessage(SendMessageEvent message)
        {
            if (!Friends.Any(f => f.UserId == message.UserToId) || _userInstance?.User == default)
                return;

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();
            var backgroundsService = scope.ServiceProvider.GetRequiredService<IBackgroundsService>();
            var task = scope.ServiceProvider.GetTask<AddPrivateChatlogTask>("AddPrivateChatlogTask");

            await scope.DisposeAsync();

            if (!usersService.Users.TryGetValue(message.UserToId, out var user))
                return;

            if (user == default)
                return;

            if (user.Client == default)
                return;

            if (user.User == default)
                return;

            var text = message.Message!;

            var emojis = GetEmojis(text);
            emojis.ForEach(emoji => text = text.Replace(emoji.Key, emoji.Value));

            await user.Client.Send(new NewConsoleMessageMessageComposer(_userInstance.User.Id, text));

            if (task != default)
            {
                task.Parameters.TryAdd("fromId", _userInstance.User.Id);
                task.Parameters.TryAdd("toId", user.User.Id);
                task.Parameters.TryAdd("message", message.Message!);
                backgroundsService.Queue(task);
            }
        }

        async Task OnFriendUpdate(FriendUpdateEvent @event)
        {
            var friend = Friends.FirstOrDefault(f => f.UserId == @event.FriendId);
            if (friend != default && @event.Client?.UserInstance?.CurrentRoomInstance != default)
            {
                friend.Client = @event.Client;
                friend.CurrentRoom = @event.Client.UserInstance.CurrentRoomInstance;

                var client = GetClient();
                if (@event.Notification && client?.UserInstance?.User != default)
                    await client.Send(new FriendListUpdateMessageComposer(client.UserInstance.User, friend, default));
            }
        }

        async Task OnFriendStatusChanged(FriendStatusChangedEvent @event)
        {
            if (_userInstance?.User == default)
                return;

            var friendIds = Friends.Select(f => f.UserId).ToList();

            var clients = Sessions.ConnectedClients.Values.Where(c => c.UserInstance?.User != default && friendIds.Contains(c.UserInstance.User.Id)).ToList();
            if (clients.Count <= 0)
                return;

            var cancellationSource = new CancellationTokenSource();
            await Parallel.ForEachAsync(clients.Where(c => c.UserInstance?.User != default).ToList(), new ParallelOptions()
            {
                CancellationToken = cancellationSource.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            }, async (client, token) =>
            {
                await client.UserInstance!.EventsService.DispatchAsync<FriendUpdateEvent>(new()
                {
                    Client = client,
                    Notification = true,
                    FriendId = _userInstance.User.Id,
                    UserId = client.UserInstance.User!.Id
                });

                await _userInstance.EventsService.DispatchAsync<FriendUpdateEvent>(new()
                {
                    Client = client,
                    Notification = @event.Notification,
                    FriendId = client.UserInstance.User!.Id,
                    UserId = _userInstance.User.Id
                });
            });
        }

        async Task OnMessengerInitMessage(MessengerInitMessageEvent _)
        {
            if (_userInstance?.Client == default || _userInstance?.User == default)
                return;

            var friends = Friends.Where(b => b.Online).ToList();

            await _userInstance.Client.Send(new MessengerInitMessageComposer());

            var page = 0;
            var pages = (friends.Count - 1) / 500 + 1;

            if (friends.Count == 0)
                await _userInstance.Client.Send(new BuddyListMessageComposer(friends, pages, page));
            else
            {
                foreach (var batch in friends.Chunk(500))
                {
                    await _userInstance.Client.Send(new BuddyListMessageComposer([.. batch], pages, page));
                    page++;
                }
            }
        }

        #region private methods 

        Client? GetClient()
            => Sessions.ConnectedClients.Values.FirstOrDefault(cc => cc.UserInstance?.User != default && _userInstance?.User != default && cc.UserInstance.User.Id == _userInstance.User.Id);

        List<KeyValuePair<string, string>> GetEmojis(string text)
            => [.. Emojis.Where(e => text.Contains(e.Key, StringComparison.InvariantCultureIgnoreCase))];

        #endregion
    }
}