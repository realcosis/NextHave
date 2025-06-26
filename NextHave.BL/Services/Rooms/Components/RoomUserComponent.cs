using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Events.Rooms;
using NextHave.BL.Messages;
using NextHave.BL.Messages.Output;
using NextHave.BL.Models;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Instances;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Rooms.Components
{
    [Service(ServiceLifetime.Scoped)]
    class RoomUserComponent(RoomUserFactory roomUserFactory) : IRoomComponent
    {
        IRoomInstance? _roomInstance;

        readonly ConcurrentDictionary<int, IRoomUserInstance> users = [];

        int virtualId = 1;

        async Task IRoomComponent.Init(IRoomInstance roomInstance)
        {
            _roomInstance = roomInstance;
            await roomInstance.EventsService.SubscribeAsync<AddUserToRoomEvent>(roomInstance, OnRequestGameMapEvent);
        }

        async Task OnRequestGameMapEvent(AddUserToRoomEvent @event)
        {
            if (@event?.User == default || _roomInstance == default || _roomInstance.RoomModel == default)
                return;

            var roomUser = roomUserFactory.GetRoomUserInstance(@event.User.Id, @event.User.Username!, virtualId++, @event.User, _roomInstance);
            roomUser.SetPosition(new Point(_roomInstance.RoomModel.DoorX, _roomInstance.RoomModel.DoorY), _roomInstance.RoomModel.DoorZ);

            users.TryAdd(roomUser.VirutalId, roomUser);

            await using var usersMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.UsersMessageComposer);
            usersMessageComposer.AddInt32(1);
            roomUser.Serialize(usersMessageComposer);
            await Send(usersMessageComposer);
        }

        async Task Send(ServerMessage message)
        {
            foreach (var client in users.Select(u => u.Value).Where(ru => ru.Client != default).Select(ru => ru.Client!))
                await client.Send(message.Bytes());
        }
    }
}