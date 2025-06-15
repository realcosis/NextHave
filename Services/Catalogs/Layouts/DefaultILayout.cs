using Dolphin.Core.Injection;
using NextHave.DAL.Mongo;
using NextHave.Messages;

namespace NextHave.Services.Catalogs.Layouts
{
    [Service(ServiceLifetime.Singleton)]
    class DefaultILayout(MongoDbContext mongoDbContext) : ICatalogLayout
    {
        async Task ICatalogLayout.SerializePage(ServerMessage message)
        {
            await Task.CompletedTask;
        }
    }
}