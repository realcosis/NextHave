using NextHave.BL.Clients;
using NextHave.BL.Messages;

namespace NextHave.BL.Services.Packets
{
    public interface IPacketsService
    {
        Task Publish<T>(T message, IClient client) where T : IMessageEvent;

        void Subscribe<T>(object subscriber, Func<T, IClient, Task> handler) where T : IMessageEvent;

        void Unsubscribe<T>(object subscriber, Func<T, IClient, Task> handler) where T : IMessageEvent;

        bool Exists<T>(object subscriber, Func<T, IClient, Task> handler) where T : IMessageEvent;
    }
}