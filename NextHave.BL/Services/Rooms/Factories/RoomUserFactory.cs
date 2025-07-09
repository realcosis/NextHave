using Dolphin.Core.Injection;
using NextHave.BL.Services.Rooms.Instances;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Services.Users.Instances;

namespace NextHave.BL.Services.Rooms.Factories
{
    [Service(ServiceLifetime.Scoped)]
    class RoomUserFactory(IKeyedServicesProvider<IRoomUserInstance> roomUserInstanceProvider)
    {
        public IRoomUserInstance GetRoomUserInstance(int userId, string username, int virtualId, IUserInstance userInstance, IRoomInstance roomInstance)
        {
            if (roomUserInstanceProvider.HasService(userId, out var roomUserInstance))
                return roomUserInstance!;

            roomUserInstance = roomUserInstanceProvider.GetService(userId);
            roomUserInstance.SetData(userId, username, virtualId, roomInstance);
            roomUserInstance.UserInstance = userInstance;
            roomUserInstance.Client = userInstance.Client;

            return roomUserInstance;
        }

        public void DestroyRoomUserInstance(int userId)
            => roomUserInstanceProvider.DestroyService(userId);
    }
}