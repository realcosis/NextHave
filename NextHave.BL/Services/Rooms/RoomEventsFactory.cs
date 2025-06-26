using Dolphin.Core.Events;
using Dolphin.Core.Injection;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Rooms
{
    [Service(ServiceLifetime.Singleton)]
    public class RoomEventsFactory(IEventsService eventsService)
    {
        readonly ConcurrentDictionary<int, RoomEventsService> roomServices = [];

        public RoomEventsService GetForRoom(int roomId)
            => roomServices.GetOrAdd(roomId, id => new RoomEventsService(eventsService)
            {
                RoomId = id
            });

        public void CleanupRoom(int roomId)
            => roomServices.TryRemove(roomId, out _);
    }
}