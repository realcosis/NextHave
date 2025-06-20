using Microsoft.JSInterop;
using NextHave.BL.Clients;
using NextHave.BL.Messages.Input;

namespace NextHave.Nitro.Clients
{
    public class Client(string sessionId) : IClient
    {
        IClient Instance => this;

        public string? SessionId { get; private set; }

        public InputHandler? Handler { get; private set; }

        public IJSRuntime? JSRuntime { get; private set; }

        public List<string> Channels { get; } = [];

        public void Init(IJSRuntime? jsRuntime)
        {
            SessionId = sessionId;
            JSRuntime = jsRuntime;
            Handler ??= new()
            {
                Client = Instance
            };
        }

        public async Task Send(string channel, byte[] output)
        {
            try
            {
                if (Channels.Contains(channel) && JSRuntime != default)
                    await JSRuntime.InvokeVoidAsync("Nitro.communication.connection.onBlazorMessage", channel, output);
            }
            catch (JSException ex)
            {
                Console.WriteLine($"JSInterop Error: {ex}");
            }
        }
    }
}