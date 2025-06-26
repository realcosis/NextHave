using Dolphin.Core.Events;
using Dolphin.Core.Injection;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Rooms.Factories
{
    [Service(ServiceLifetime.Singleton)]
    public class RoomEventsFactory(IEventsService eventsService)
    {
        readonly ConcurrentDictionary<int, RoomEventsService> roomEventsServices = [];

        public RoomEventsService GetForRoom(int roomId)
            => roomEventsServices.GetOrAdd(roomId, id => new RoomEventsService(eventsService)
            {
                RoomId = id
            });

        public void CleanupRoom(int roomId)
            => roomEventsServices.TryRemove(roomId, out _);
    }
}