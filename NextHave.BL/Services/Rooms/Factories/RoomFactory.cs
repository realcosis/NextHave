using Dolphin.Core.Injection;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Services.Rooms.Instances;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Rooms.Factories
{
    [Service(ServiceLifetime.Scoped)]
    class RoomFactory(IKeyedServicesProvider<IRoomInstance> roomInstanceProvider)
    {
        public (IRoomInstance roomInstance, bool firstLoad) GetRoomInstance(int roomId, Room room)
        {
            if (roomInstanceProvider.HasServiceRegstered(roomId, out var roomInstance))
                return (roomInstance!, false);

            roomInstance = roomInstanceProvider.GetRequiredKeyedService(roomId);
            roomInstance.Room = room;

            return (roomInstance, true);
        }

        public void DestroyRoomInstance(int userId)
            => roomInstanceProvider.Disable(userId);
    }
}