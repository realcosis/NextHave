using Microsoft.AspNetCore.SignalR;
using NextHave.BL.Messages;
using NextHave.BL.Messages.Input;
using NextHave.BL.Messages.Output.Rooms.Notifications;
using NextHave.BL.Services.Users.Instances;

namespace NextHave.BL.Clients
{
    public class Client
    {
        public string? ConnectionId { get; private set; }

        public InputHandler? Handler { get; set; }

        public IClientProxy? ClientProxy { get; set; }

        public IUserInstance? UserInstance { get; set; }

        public void Init(string connectionId, IClientProxy clientProxy)
        {
            ConnectionId = connectionId;
            ClientProxy = clientProxy;
            Handler ??= new()
            {
                Client = this
            };
        }

        public async Task SendSystemNotification(string type, Dictionary<string, string> items)
        {
            items.TryAdd("sound", "systemnotification");
            items.TryAdd("color", "#ff0000");

            await SendBubble(type, items);
        }

        public async Task SendBubble(string type, Dictionary<string, string> items)
        {
            items.TryAdd("display", "BUBBLE");
            await Send(new BubbleNotificationsMessageComposer(type, items));
        }

        public async Task Send(Composer message)
        {
            if (ClientProxy != default)
            {
                var output = message.Write().Bytes();
                await ClientProxy.SendAsync("ReceiveBinary", output);
            }
        }
    }
}