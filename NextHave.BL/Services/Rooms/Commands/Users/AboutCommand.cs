using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Clients;
using NextHave.BL.Messages.Output.Rooms.Notifications;
using NextHave.BL.Services.Items;
using NextHave.BL.Services.Texts;
using NextHave.BL.Services.Users;
using System.Diagnostics;
using System.Text;

namespace NextHave.BL.Services.Rooms.Commands.Users
{
    [Service(ServiceLifetime.Singleton)]
    class AboutCommand(ITextsService textsService, IRoomsService roomsService, IUsersService usersService, IItemsService itemsService) : ChatCommand
    {
        public override string? Key => "info";

        public override string[] OtherKeys => [ "about", "infomation" ];

        public override string? Description => textsService.GetText("chatcommand_info_description", "Displays generic information that everybody loves to see.");

        public override string? Usage => string.Empty;

        protected override async Task Handle(Client client)
        {
            var uptime = DateTime.Now - ProjectConstants.StartDate;
            var userCount = usersService.Users.Count;
            var roomCount = roomsService.ActiveRooms.Count;
            var itemCount = itemsService.ItemDefinitions.Count;

            var text = new StringBuilder();

            text.AppendLine("<b><font size=\"5\" color=\"#0B615E\">NextHave</font><br><font size=\"3\" color=\"#ff5733\">Developed by Charlotte</font></b><br/>");
            text.AppendLine($"<b>Build:</b> {ProjectConstants.Build}<br/>");
            text.AppendLine("<b>Statistiche:</b>");
            text.AppendLine($"Avvio: {uptime.Days} giorni, {uptime.Hours} ore, {uptime.Minutes} minuti e {uptime.Seconds} secondi");
            text.AppendLine($"{userCount} utenti online");
            text.AppendLine($"{roomCount} stanze attive");
            text.AppendLine($"{itemCount} furni caricati<br/>");

            await client.Send(new RoomNotificationMessageComposer("Informazioni", text.ToString(), string.Empty));
        }
    }
}