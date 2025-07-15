using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Clients;
using NextHave.BL.Messages.Output.Navigators;
using NextHave.BL.Services.Texts;
using System.Text;

namespace NextHave.BL.Services.Rooms.Commands.Users
{
    [Service(ServiceLifetime.Singleton)]
    class CommandsCommand(ITextsService textsService, IServiceScopeFactory serviceScopeFactory) : ChatCommand
    {
        public override string? Key => "commands";

        public override string[] OtherKeys => ["comandi"];

        public override string? Description => textsService.GetText("chatcommand_commands_description", "Show all of yours comamnds.");

        public override string? Usage => string.Empty;

        public override string? Permission => string.Empty;

        protected override async Task Handle(Client client)
        {
            if (client.UserInstance?.Permission == default)
                return;

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var commands = scope
                            .ServiceProvider
                                .GetRequiredService<IEnumerable<IChatCommand>>()
                                    .Where(c => string.IsNullOrWhiteSpace(c.Permission) || client.UserInstance.Permission.Rights.Contains(c.Permission))
                                    .ToList();

            var columns = new List<string>()
            {
                textsService.GetText("command_column_key", "Command"),
                textsService.GetText("command_usage_key", "Usage"),
                textsService.GetText("command_description_key", "Description")
            };

            var rows = new Dictionary<int, Dictionary<int, string>>();
            foreach (var (index, command) in commands.Index())
            {
                var value = new Dictionary<int, string>
                {
                    { 0, command.Key! },
                    { 1, command.Usage! },
                    { 2, command.Description! }
                };
                rows.Add(index, value);
            }

            await client.Send(new TableAlertMessageComposer(columns, rows));

            await scope.DisposeAsync();
        }
    }
}