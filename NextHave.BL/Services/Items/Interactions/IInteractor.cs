using NextHave.BL.Messages;
using NextHave.BL.Models.Items;

namespace NextHave.BL.Services.Items.Interactions
{
    public interface IInteractor
    {
        bool EnableSerialize { get; }

        Task Execute();

        void Serialize(ServerMessage message, RoomItem item, int userId);
    }
}