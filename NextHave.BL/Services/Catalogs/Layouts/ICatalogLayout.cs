using NextHave.DAL.Mongo.Entities;
using NextHave.BL.Messages;

namespace NextHave.BL.Services.Catalogs.Layouts
{
    public interface ICatalogLayout
    {
        string Type { get; }

        CatalogPageEntity? Entity { get; set; }

        Task SerializePage(ServerMessage message);
    }
}