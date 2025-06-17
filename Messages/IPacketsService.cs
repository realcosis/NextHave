using NextHave.Clients;

namespace NextHave.Messages
{
    public interface IPacketsService
    {
        Task Publish<T>(T message, Client client) where T : IMessageEvent;

        void Subscribe<T>(object subscriber, Func<T, Client, Task> handler) where T : IMessageEvent;

        void Unsubscribe<T>(object subscriber, Func<T, Client, Task> handler) where T : IMessageEvent;

        bool Exists<T>(object subscriber, Func<T, Client, Task> handler) where T : IMessageEvent;
    }
}