using NextHave.DAL.Mongo;
using NextHave.DAL.MySQL;
using Dolphin.Core.Injection;
using NextHave.BL.Services.Users.Instances;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Users.Components
{
    [Service(ServiceLifetime.Scoped)]
    class InventoryComponent(IServiceScopeFactory serviceScopeFactory) : IUserComponent
    {
        async Task IUserComponent.Init(IUserInstance userInstance)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();
        }
    }
}