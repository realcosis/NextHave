using Dolphin.Core.Backgrounds;
using Dolphin.Core.Backgrounds.Tasks;
using Dolphin.Core.Enums;
using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.DAL.MySQL;
using System.Collections.Concurrent;

namespace Dolphin.Backgrounds.Tasks
{
    [Service(ServiceLifetime.Scoped, Keyed = true, Key = "AddCatalogTask")]
    class AddCatalogTask(IServiceScopeFactory serviceScopeFactory) : ITask
    {
        public ConcurrentDictionary<string, object> Parameters { get; set; } = [];

        public async Task Execute()
        {
            if (Parameters.IsEmpty)
                return;

            var message = Parameters.GetParameter<string>("message", DolphinTypeCode.String);
            
            var roomId = Parameters.GetParameter<int>("roomId", DolphinTypeCode.Int32);

            var userId = Parameters.GetParameter<int>("userId", DolphinTypeCode.Int32);

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            await scope.DisposeAsync();

            await mysqlDbContext.ChatlogRooms.AddAsync(new()
            {
                Datetime = DateTime.Now,
                Message = message,
                RoomId = roomId,
                UserId = userId
            });
            await mysqlDbContext.SaveChangesAsync();
        }
    }
}