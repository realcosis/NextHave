using Dolphin.Core.Events;
using NextHave.BL.Events.Rooms;

namespace NextHave.BL.Services.Rooms
{
    public class RoomEventsService(IEventsService eventsService)
    {
        bool hasRoomId = false;
        int roomId;

        public int RoomId
        {
            get => roomId;
            set
            {
                if (!hasRoomId)
                {
                    roomId = value;
                    hasRoomId = true;
                }
            }
        }

        public async Task SubscribeAsync<T>(object subscriber, Func<T, Task> handler) where T : DolphinEvent
            => await eventsService.SubscribeAsync<T>(subscriber, async evt =>
               {
                   if (evt is RoomEvent roomEvent && roomEvent.RoomId == RoomId)
                       await handler(evt);
               });

        public async Task UnsubscribeAsync<T>(object subscriber, Delegate handler) where T : DolphinEvent
            => await eventsService.UnsubscribeAsync<T>(subscriber, handler);

        public async Task<T> DispatchAsync<T>(T message) where T : DolphinEvent
        {
            if (message is RoomEvent roomEvent)
                roomEvent.RoomId = roomId;

            return await eventsService.DispatchAsync(message);
        }
    }
}