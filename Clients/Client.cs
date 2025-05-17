using Microsoft.JSInterop;
using NextHave.Messages.Input;

namespace NextHave.Clients
{
    public class Client(string sessionId)
    {
        public string? SessionId { get; private set; }

        public InputHandler? Handler { get; private set; }

        public IJSRuntime? JSRuntime { get; private set; }

        public readonly List<string> Channels = [];

        public void Init(IJSRuntime? jsRuntime)
        {
            SessionId = sessionId;
            JSRuntime = jsRuntime;
            Handler ??= new()
            {
                Client = this
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