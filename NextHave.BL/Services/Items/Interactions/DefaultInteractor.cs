using NextHave.DAL.Enums;
using NextHave.BL.Messages;
using NextHave.BL.Attributes;
using Dolphin.Core.Injection;
using NextHave.BL.Models.Items;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Items.Interactions
{
    [Interactor(InteractionTypes.Default)]
    [Service(ServiceLifetime.Singleton, Keyed = true, Key = nameof(DefaultInteractor))]
    class DefaultInteractor : IInteractor
    {
        bool IInteractor.EnableSerialize => false;

        Task IInteractor.Execute()
        {
            throw new NotImplementedException();
        }

        void IInteractor.Serialize(ServerMessage message, RoomItem item, int userId)
        {
            throw new NotImplementedException();
        }
    }
}