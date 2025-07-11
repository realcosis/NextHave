using System.Text;
using NextHave.BL.Clients;
using Dolphin.Core.Injection;
using NextHave.BL.Services.Texts;
using NextHave.BL.Services.Items;
using NextHave.BL.Services.Users;
using NextHave.BL.Services.Settings;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Messages.Output.Rooms.Notifications;

namespace NextHave.BL.Services.Rooms.Commands.Users
{
    [Service(ServiceLifetime.Singleton)]
    class AboutCommand(ITextsService textsService, ISettingsService settingsService, IRoomsService roomsService, IUsersService usersService, IItemsService itemsService) : ChatCommand
    {
        public override string? Key => "info";

        public override string[] OtherKeys => ["about", "infomation"];

        public override string? Description => textsService.GetText("chatcommand_info_description", "Displays generic information that everybody loves to see.");

        public override string? Usage => string.Empty;

        protected override async Task Handle(Client client)
        {
            var uptime = DateTime.Now - ProjectConstants.StartDate;
            var userCount = usersService.Users.Count;
            var roomCount = roomsService.ActiveRooms.Count;
            var itemCount = itemsService.ItemDefinitions.Count;

            var text = new StringBuilder();

            text.AppendLine("<b><font size=\"5\" color=\"#ff5733\">Made by NextHave</font></b><br/>");
            text.AppendLine($"<b>Build:</b> {ProjectConstants.Build}<br/>");
            text.AppendLine("<b>Statistiche:</b>");
            text.AppendLine($"Avvio - {uptime.Days} giorni, {uptime.Hours} ore e {uptime.Minutes} minuti");
            text.AppendLine($"Utenti online - {userCount} utenti online");
            text.AppendLine($"Stanze attive - {roomCount} stanze attive");
            text.AppendLine($"Furni caricati - {itemCount}<br/>");
            text.AppendLine("<b>Perfomance:</b>");
            text.AppendLine($"Processori logici - {Environment.ProcessorCount}");
            text.AppendLine($"Memoria totale -  {MegabytesToGigabytes(BytesToMegabytes(ProjectConstants.HardwareInfo!.MemoryStatus.TotalPhysical))}GB");
            text.AppendLine($"Memoria usata - {BytesToMegabytes(GC.GetTotalMemory(false))}/{BytesToMegabytes(ProjectConstants.HardwareInfo!.MemoryStatus.TotalPhysical)}MB");

            await client.Send(new RoomNotificationMessageComposer($"{settingsService.GetSetting("hotel_name")} Hotel", text.ToString(), "butterflyemulator", "https://i.imgur.com/I6mJiLk.png"));
        }

        static int BytesToMegabytes(long bytes)
            => (int)(bytes / 1024f / 1024f);

        static int BytesToMegabytes(ulong bytes)
            => (int)(bytes / 1024f / 1024f);

        static int MegabytesToGigabytes(int megabytes)
            => (int)(megabytes / 1024.0);
    }
}