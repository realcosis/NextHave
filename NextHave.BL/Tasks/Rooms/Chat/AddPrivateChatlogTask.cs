using Dolphin.Core.Backgrounds;
using Dolphin.Core.Backgrounds.Tasks;
using Dolphin.Core.Enums;
using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextHave.DAL.MySQL;
using NextHave.DAL.MySQL.Entities;
using System.Collections.Concurrent;

namespace NextHave.BL.Tasks.Rooms.Chat
{
    [Service(ServiceLifetime.Scoped, Keyed = true, Key = "AddPrivateChatlogTask")]
    class AddPrivateChatlogTask(IServiceScopeFactory serviceScopeFactory) : ITask
    {
        public ConcurrentDictionary<string, object> Parameters { get; set; } = [];

        public async Task Execute()
        {
            if (Parameters.IsEmpty)
                return;

            var fromId = Parameters.GetParameter<int>("fromId", DolphinTypeCode.Int32);
            var toId = Parameters.GetParameter<int>("toId", DolphinTypeCode.Int32);
            var message = Parameters.GetParameter<string>("message", DolphinTypeCode.String);

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            await scope.DisposeAsync();

            var privateChatlog = await mysqlDbContext.ChatlogPrivates.FirstOrDefaultAsync(cp => cp.FromId == fromId && cp.ToId == toId || cp.FromId == toId && cp.ToId == fromId);
            if (privateChatlog == default)
            {
                privateChatlog = new ChatlogPrivateEntity
                {
                    FromId = fromId,
                    ToId = toId
                };
                await mysqlDbContext.ChatlogPrivates.AddAsync(privateChatlog);
            }
            var entity = new ChatlogPrivateDetailEntity
            {
                Chatlog = privateChatlog,
                Message = message,
                Timestamp = DateTime.Now,
                UserId = fromId
            };
            await mysqlDbContext.ChatlogPrivateDetails.AddAsync(entity);
            await mysqlDbContext.SaveChangesAsync();
        }
    }
}