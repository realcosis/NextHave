using Dolphin.Core.Injection;
using NextHave.DAL.Mongo.Entities;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Messages;

namespace NextHave.BL.Services.Catalogs.Layouts
{
    [Service(ServiceLifetime.Singleton, Keyed = true, Key = "Default3x3Layout")]
    class Default3x3Layout(IServiceProvider serviceProvider) : ICatalogLayout
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