using Dolphin.Core.Enums;
using NextHave.BL.Clients;
using Dolphin.Core.Injection;
using Dolphin.Core.Backgrounds;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Services.Rooms;
using System.Collections.Concurrent;
using Dolphin.Core.Backgrounds.Tasks;
using NextHave.BL.Models.Rooms.Navigators;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Tasks.Rooms.Settings
{
    [Service(ServiceLifetime.Scoped, Keyed = true, Key = "SaveRoomSettingsTask")]
    class SaveRoomSettingsTask(IServiceScopeFactory serviceScopeFactory) : ITask
    {
        public ConcurrentDictionary<string, object> Parameters { get; set; } = [];

        public async Task Execute()
        {
            if (Parameters.IsEmpty)
                return;

            var room = Parameters.GetParameter<Room>("room", DolphinTypeCode.Custom);
            var clientId = Parameters.GetParameter<string>("client", DolphinTypeCode.String);
            var category = Parameters.GetParameter<NavigatorCategory>("category", DolphinTypeCode.Custom);

            if (!Sessions.ConnectedClients.TryGetValue(clientId!, out var client))
                return;

            if (room == default || client == default || category == default)
                return;

            var roomsService = await serviceScopeFactory.GetRequiredService<IRoomsService>();

            await roomsService.SaveRoom(room, client, category);
        }
    }
}