using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Events.Rooms.Engine;

namespace NextHave.BL.Services.Rooms.Components
{
    [Service(ServiceLifetime.Scoped)]
    class GameMapComponent(IRoomsService roomsService) : IRoomComponent
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

            var roomModel = await roomsService.GetRoomModel(_roomInstance.Room.ModelName!, _roomInstance.Room.Id);
            if (roomModel != default)
            {
                roomModel.RoomInstance = _roomInstance;
                _roomInstance.RoomModel = new WorkRoomModel(_roomInstance, roomModel);
            }
        }
    }
}