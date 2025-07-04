using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Clients;

namespace NextHave.BL.Services.Rooms.Commands
{
    public static class ChatCommandHandler
    {
        public static async Task InvokeCommand(this string? inputData, IServiceProvider serviceProvider, Client client)
        {
            if (string.IsNullOrWhiteSpace(inputData))
                return;

            var parameters = inputData.Replace(":", string.Empty).Split(' ');
            
            var commands = serviceProvider.GetRequiredService<IEnumerable<IChatCommand>>();

            var command = commands.FirstOrDefault(c => c.Key!.ToLower().Equals(parameters[0].ToLower(), StringComparison.InvariantCultureIgnoreCase) || c.OtherKeys.Any(k => k.Equals(parameters[0].ToLower(), StringComparison.InvariantCultureIgnoreCase)));

            if (command != default)
            {
                command.Parameters = [.. parameters.Skip(1)];
                await command.Execute(client);
            }
        }
    }
}