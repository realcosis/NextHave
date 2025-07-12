using Dolphin.Core.Injection;
using NextHave.BL.Services.Rooms;
using System.Collections.Concurrent;
using Dolphin.Core.Backgrounds.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Tasks.Rooms
{
    [Service(ServiceLifetime.Scoped, Keyed = true, Key = "RoomTickTask")]
    class RoomTickTask(IServiceScopeFactory serviceScopeFactory) : ITask
    {
        public ConcurrentDictionary<string, object> Parameters { get; set; } = [];

        public async Task Execute()
        {
            var cancellationSource = new CancellationTokenSource();
            var roomsService = await serviceScopeFactory.GetRequiredService<IRoomsService>();

            await Parallel.ForEachAsync(roomsService.ActiveRooms.Values, new ParallelOptions()
            {
                CancellationToken = cancellationSource.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            }, async (roomInstance, token) => await roomInstance.Tick());
        }
    }
}