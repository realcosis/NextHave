using Dolphin.Core.Events;
using NextHave.BL.Events.Users;

namespace NextHave.BL.Services.Rooms
{
    public class UserEventsService(IEventsService eventsService)
    {
        bool hasUserId = false;
        int userId;

        public int UserId
        {
            get => userId;
            set
            {
                if (!hasUserId)
                {
                    userId = value;
                    hasUserId = true;
                }
            }
        }

        public async Task SubscribeAsync<T>(object subscriber, Func<T, Task> handler) where T : DolphinEvent
            => await eventsService.SubscribeAsync<T>(subscriber, async evt =>
               {
                   if (evt is UserEvent userEvent && userEvent.UserId == UserId)
                       await handler(evt);
               });

        public async Task UnsubscribeAsync<T>(object subscriber, Func<T, Task> handler) where T : DolphinEvent
            => await eventsService.UnsubscribeAsync<T>(subscriber, handler);

        public async Task<T> DispatchAsync<T>(T message) where T : DolphinEvent
        {
            if (message is UserEvent userEvent)
                userEvent.UserId = userId;

            return await eventsService.DispatchAsync(message);
        }
    }
}