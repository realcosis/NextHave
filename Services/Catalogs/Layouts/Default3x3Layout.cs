using Dolphin.Core.Injection;
using NextHave.DAL.Mongo;
using NextHave.DAL.Mongo.Entities;
using NextHave.Messages;

namespace NextHave.Services.Catalogs.Layouts
{
    [Service(ServiceLifetime.Singleton)]
    class Default3x3Layout(MongoDbContext mongoDbContext) : ICatalogLayout
    {
        ICatalogLayout Instance => this;

        string ICatalogLayout.Type => "default_3x3";

        CatalogPageEntity? ICatalogLayout.Entity { get; set; }

        async Task ICatalogLayout.SerializePage(ServerMessage message)
        {
            if (Instance.Entity == default)
                return;

            message.AddString(Instance.Type);
            await Task.CompletedTask;
        }
    }
}