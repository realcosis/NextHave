using Dolphin.Core.Injection;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Rooms.Instances;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Rooms.Factories
{
    [Service(ServiceLifetime.Scoped)]
    class RoomUserFactory(IKeyedServicesProvider<IRoomUserInstance> roomUserInstanceProvider)
    {
        public IRoomUserInstance GetRoomUserInstance(int userId, string username, int virtualId, User user, IRoomInstance roomInstance)
        {
            if (roomUserInstanceProvider.HasServiceRegstered(userId, out var roomUserInstance))
                return roomUserInstance!;

            roomUserInstance = roomUserInstanceProvider.GetRequiredKeyedService(userId);
            roomUserInstance.SetData(userId, username, virtualId, roomInstance);
            roomUserInstance.User = user;
            roomUserInstance.Client = user.Client;

            return roomUserInstance;
        }

        public void DestroyRoomUserInstance(int userId)
            => roomUserInstanceProvider.Disable(userId);
    }
}