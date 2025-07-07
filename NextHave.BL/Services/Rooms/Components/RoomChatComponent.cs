using Dolphin.Backgrounds.Tasks;
using Dolphin.Core.Backgrounds;
using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Events.Rooms.Chat;
using NextHave.BL.Events.Rooms.Session;
using NextHave.BL.Messages.Output.Rooms.Chat;
using NextHave.BL.Services.Rooms.Commands;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Services.Texts;
using NextHave.BL.Services.Users.Factories;
using NextHave.DAL.MySQL;

namespace NextHave.BL.Services.Rooms.Components
{
    [Service(ServiceLifetime.Scoped)]
    class RoomChatComponent(IServiceScopeFactory serviceScopeFactory, UserFactory userFactory) : IRoomComponent
    {
        IRoomInstance? _roomInstance;

        readonly Dictionary<string, string> emojis = [];

        async Task IRoomComponent.Dispose()
        {
            if (_roomInstance == default)
                return;

            await _roomInstance.EventsService.UnsubscribeAsync<ChatMessageEvent>(_roomInstance, OnChatMessage);
            _roomInstance = default;
        }

        async Task IRoomComponent.Init(IRoomInstance roomInstance)
        {
            _roomInstance = roomInstance;

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            var dbEmojis = await mysqlDbContext.RoomEmojis.AsNoTracking().ToListAsync();
            dbEmojis.ForEach(emoji => emojis.TryAdd(emoji.Code!, emoji.Emoji!));

            await _roomInstance.EventsService.SubscribeAsync<ChatMessageEvent>(_roomInstance, OnChatMessage);
        }

        async Task OnChatMessage(ChatMessageEvent @event)
        {
            if (_roomInstance?.Room == default)
                return;

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var backgroundsService = scope.ServiceProvider.GetRequiredService<IBackgroundsService>();
            var textsService = scope.ServiceProvider.GetRequiredService<ITextsService>();

            var userInstance = userFactory.GetUserInstance(@event.UserId);

            if (userInstance?.Client == default)
                return;

            if (!userInstance.Permission!.HasRight("nexthave_room_bypass_mute") && _roomInstance.CheckMute(@event.VirtualId, userInstance))
            {
                await userInstance.Client!.SendSystemNotification("generic", new()
                {
                    ["message"] = textsService.GetText("nexthave_roomuser_muted", "You are muted in this room.")
                });
                return;
            }

            var task = scope.ServiceProvider.GetTask<AddCatalogTask>("AddCatalogTask");
            if (task == default)
                return;

            var message = @event.Message!.Length > 100 ? @event.Message[..100] : @event.Message;

            var emojis = GetEmojis(message);
            emojis.ForEach(emoji => message = message.Replace(emoji.Key, emoji.Value));

            if (message.StartsWith(':'))
            {
                await ChatCommandHandler.InvokeCommand(message, scope.ServiceProvider, userInstance.Client);
                return;
            }

            task.Parameters.TryAdd("message", @event.Message);
            task.Parameters.TryAdd("roomId", _roomInstance.Room.Id);
            task.Parameters.TryAdd("userId", @event.UserId);
            backgroundsService.Queue(task);

            if (@event.Shout)
                await _roomInstance.EventsService.DispatchAsync<SendRoomPacketEvent>(new()
                {
                    Composer = new ShoutMessageMessageComposer(@event.VirtualId, message, 0, @event.Color),
                    WithRights = false,
                    RoomId = _roomInstance.Room.Id,
                });
            else
                await _roomInstance.EventsService.DispatchAsync<SendRoomPacketEvent>(new()
                {
                    Composer = new ChatMessageMessageComposer(@event.VirtualId, message, 0, @event.Color),
                    WithRights = false,
                    RoomId = _roomInstance.Room.Id,
                });
        }

        #region private methods 

        List<KeyValuePair<string, string>> GetEmojis(string text)
            => [.. emojis.Where(e => text.Contains(e.Key, StringComparison.InvariantCultureIgnoreCase))];

        #endregion
    }
}