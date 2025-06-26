using Dolphin.Core.Injection;
using NextHave.BL.Models.Rooms;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Services.Rooms.Factories
{
    [Service(ServiceLifetime.Scoped)]
    class RoomFactory(IKeyedServicesProvider<IRoomInstance> roomInstanceProvider)
    {
        public IRoomInstance GetRoomInstance(int roomId, Room room)
        {
            if (roomInstanceProvider.HasServiceRegstered(roomId, out var roomInstance))
                return roomInstance!;

            roomInstance = roomInstanceProvider.GetRequiredKeyedService(roomId);
            roomInstance.Room = room;

            return roomInstance;
        }
    }
}