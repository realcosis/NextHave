using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Events.Rooms.Engine;

namespace NextHave.BL.Services.Rooms.Components
{
    [Service(ServiceLifetime.Scoped)]
    class GameMapComponent(IServiceScopeFactory serviceScopeFactory) : IRoomComponent
    {
        IRoomInstance? _roomInstance;

        async Task IRoomComponent.Dispose()
        {
            if (_roomInstance == default)
                return;

            await _roomInstance.EventsService.UnsubscribeAsync<RequestRoomGameMapEvent>(_roomInstance, OnRequestGameMapEvent);
            _roomInstance = default;
        }

        async Task IRoomComponent.Init(IRoomInstance roomInstance)
        {
            _roomInstance = roomInstance;
            await _roomInstance.EventsService.SubscribeAsync<RequestRoomGameMapEvent>(roomInstance, OnRequestGameMapEvent);
        }

        async Task OnRequestGameMapEvent(RequestRoomGameMapEvent @event)
        {
            if (_roomInstance?.Room == default)
                return;

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var roomsService = scope.ServiceProvider.GetRequiredService<IRoomsService>();
            var roomModel = await roomsService.GetRoomModel(_roomInstance.Room.ModelName!, _roomInstance.Room.Id);
            if (roomModel != default)
                _roomInstance.RoomModel = new WorkRoomModel(_roomInstance, roomModel);

            await scope.DisposeAsync();
        }
    }
}