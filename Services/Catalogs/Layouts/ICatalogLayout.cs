using NextHave.Messages;

namespace NextHave.Services.Catalogs.Layouts
{
    public interface ICatalogLayout
    {
        Task SerializePage(ServerMessage message);
    }
}