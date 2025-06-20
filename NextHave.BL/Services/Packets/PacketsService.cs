using Dolphin.Core.Events;
using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Clients;
using NextHave.BL.Messages;
using System.Reflection;

namespace NextHave.BL.Services.Packets
{
    [Service(ServiceLifetime.Singleton)]
    class PacketsService : IPacketsService
    {
        readonly List<ListenerRef> listeners = [];

        readonly object listenerLock = new();

        bool IPacketsService.Exists<T>(object subscriber, Func<T, IClient, Task> handler)
        {
            lock (listenerLock)
                return listeners.Any(x => Equals(x.Sender, subscriber) && typeof(T).Equals(x.Type) && x.Action!.Equals(handler));
        }

        async Task IPacketsService.Publish<T>(T message, IClient IClient)
        {
            foreach (var listener in GetAliveHandlers<T>())
            {
                switch (listener.Action)
                {
                    case Action<T, IClient> action:
                        action(message, IClient);
                        break;
                    case Func<T, IClient, Task> func:
                        await func(message, IClient);
                        break;
                }
            }
        }

        void IPacketsService.Subscribe<T>(object subscriber, Func<T, IClient, Task> handler)
        {
            var item = new ListenerRef
            {
                Action = handler,
                Sender = new WeakReference(subscriber),
                Type = typeof(T)
            };

            lock (listenerLock)
                listeners.Add(item);
        }

        void IPacketsService.Unsubscribe<T>(object subscriber, Func<T, IClient, Task> handler)
        {
            lock (listenerLock)
            {
                var query = listeners
                                .Where(a => !a.Sender!.IsAlive ||
                                             a.Sender!.Target!.Equals(subscriber) && a.Type == typeof(T));

                if (handler != default)
                    query = query.Where(a => a.Action!.Equals(handler));

                foreach (var h in query)
                    listeners.Remove(h);
            }
        }

        #region private methods

        List<ListenerRef> GetAliveHandlers<T>() where T : IMessageEvent
        {
            PruneHandlers();
            return listeners.Where(h => h.Type!.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo())).ToList();
        }

        void PruneHandlers()
        {
            lock (listenerLock)
                listeners.RemoveAll(x => !x.Sender!.IsAlive);
        }

        #endregion
    }
}