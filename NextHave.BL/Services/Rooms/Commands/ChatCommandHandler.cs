using NextHave.BL.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Rooms.Commands
{
    public static class ChatCommandHandler
    {
        public static async Task<bool> InvokeCommand(this string? inputData, IServiceScopeFactory serviceScopeFactory, Client client)
        {
            if (string.IsNullOrWhiteSpace(inputData) || client.UserInstance?.Permission == default)
                return false;

            var parameters = inputData.Replace(":", string.Empty).Split(' ');

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var commands = scope.ServiceProvider.GetRequiredService<IEnumerable<IChatCommand>>();

            var command = commands
                            .Where(c => string.IsNullOrWhiteSpace(c.Permission) || client.UserInstance.Permission.Rights.Contains(c.Permission))
                            .FirstOrDefault(c => c.Key!.ToLower().Equals(parameters[0].ToLower(), StringComparison.InvariantCultureIgnoreCase) || c.OtherKeys.Any(k => k.Equals(parameters[0].ToLower(), StringComparison.InvariantCultureIgnoreCase)));

            if (command != default)
            {
                command.Parameters = [.. parameters.Skip(1)];
                await command.Execute(client);
                return true;
            }

            await scope.DisposeAsync();
            return false;
        }
    }
}