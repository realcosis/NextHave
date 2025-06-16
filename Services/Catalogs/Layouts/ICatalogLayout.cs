using NextHave.DAL.Mongo.Entities;
using NextHave.Messages;

namespace NextHave.Services.Catalogs.Layouts
{
    public interface ICatalogLayout
    {
        string Type { get; }

        CatalogPageEntity? Entity { get; set; }

        Task SerializePage(ServerMessage message);
    }
}