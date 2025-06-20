using NextHave.BL.Clients;
using NextHave.BL.Messages;

namespace NextHave.BL.Services.Packets
{
    public interface IPacketsService
    {
        Task Publish<T>(T message, Client client) where T : IInput;

        void Subscribe<T>(object subscriber, Func<T, Client, Task> handler) where T : IInput;

        void Unsubscribe<T>(object subscriber, Func<T, Client, Task> handler) where T : IInput;

        bool Exists<T>(object subscriber, Func<T, Client, Task> handler) where T : IInput;
    }
}