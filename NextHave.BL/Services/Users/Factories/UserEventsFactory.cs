using Dolphin.Core.Events;
using Dolphin.Core.Injection;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Rooms.Factories
{
    [Service(ServiceLifetime.Singleton)]
    public class UserEventsFactory(IEventsService eventsService)
    {
        readonly ConcurrentDictionary<int, UserEventsService> userEventsServices = [];

        public UserEventsService Get(int userId)
            => userEventsServices.GetOrAdd(userId, id => new UserEventsService(eventsService)
            {
                UserId = id
            });

        public void CleanupUser(int userId)
            => userEventsServices.TryRemove(userId, out _);
    }
}