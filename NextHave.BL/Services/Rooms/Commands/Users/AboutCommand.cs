using System.Text;
using NextHave.BL.Clients;
using Dolphin.Core.Injection;
using NextHave.BL.Services.Texts;
using NextHave.BL.Services.Users;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Messages.Output.Rooms.Notifications;

namespace NextHave.BL.Services.Rooms.Commands.Users
{
    [Service(ServiceLifetime.Singleton)]
    class AboutCommand(ITextsService textsService, IRoomsService roomsService, IUsersService usersService) : ChatCommand
    {
        public override string? Key => "info";

        public override string[] OtherKeys => [ "about", "infomation" ];

        public override string? Description => textsService.GetText("chatcommand_info_description", "Displays generic information that everybody loves to see.");

        public override string? Usage => string.Empty;

        protected override async Task Handle(Client client)
        {
            var uptime = DateTime.Now - ServerInfo.StartDate;
            var userCount = Sessions.ConnectedClients.Count;
            var roomCount = roomsService.ActiveRooms.Count;

            var text = new StringBuilder();

            text.AppendLine("<b><font size=\"5\" color=\"#71498a\">NextHave<br>Developed by Charlotte</font></b><br/><br/>");
            text.AppendLine("<b>Statistiche:</b><br/>");
            text.AppendLine($"<b>Uptime:</b> {uptime.Days} giorni, {uptime.Hours} ore, {uptime.Minutes} minuti<br/>");
            text.AppendLine($"{userCount} <b>utenti online</b><br/>");
            text.AppendLine($"{roomCount} <b>stanze attive</b><br/>");

            await client.Send(new RoomNotificationMessageComposer("Informaizoni", text.ToString(), "butterflyemulator"));
        }
    }
}