using System.Text;
using NextHave.BL.Clients;
using Dolphin.Core.Injection;
using NextHave.BL.Services.Texts;
using NextHave.BL.Services.Rooms.Factories;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Messages.Output.Rooms.Notifications;

namespace NextHave.BL.Services.Rooms.Commands.Users
{
    [Service(ServiceLifetime.Singleton)]
    class CoordCommand(ITextsService textsService, IServiceScopeFactory serviceScopeFactory) : ChatCommand
    {
        public override string? Key => "coord";

        public override string[] OtherKeys => [];

        public override string? Description => textsService.GetText("chatcommand_coord_description", "Show your current coordinates.");

        public override string? Usage => string.Empty;

        protected override async Task Handle(Client client)
        {
            var text = new StringBuilder();

            var roomUserFactory = await serviceScopeFactory.GetRequiredService<RoomUserFactory>();
            var roomUserInstance = roomUserFactory.GetRoomUserInstance(client.UserInstance!.User!.Id);
            if (roomUserInstance == default)
                return;

            text.AppendLine($"<b>X</b>: {roomUserInstance.Position!.GetX}");
            text.AppendLine($"<b>Y</b>: {roomUserInstance.Position!.GetY}");
            text.AppendLine($"<b>Z</b>: {roomUserInstance.Position!.GetZ}");

            await client.Send(new RoomNotificationMessageComposer($"Le tue coorindate", text.ToString(), string.Empty));
        }
    }
}