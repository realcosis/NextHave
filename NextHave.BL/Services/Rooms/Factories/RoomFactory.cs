using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Models;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Services.Rooms.Factories
{
    [Service(ServiceLifetime.Scoped)]
    class RoomFactory(IKeyedServicesProvider<IRoomInstance> roomInstanceProvider)
    {
        public IRoomInstance? GetRoomInstance(int roomId)
        {
            if (roomInstanceProvider.HasService(roomId, out var roomInstance))
                return roomInstance;

            return default;
        }

        public TryLoadReference<IRoomInstance> GetRoomInstance(int roomId, Room room)
        {
            if (roomInstanceProvider.HasService(roomId, out var roomInstance))
                return new()
                {
                    FirstLoad = false,
                    Reference = roomInstance
                };

            roomInstance = roomInstanceProvider.GetService(roomId);
            roomInstance.Room = room;

            return new()
            {
                Reference = roomInstance,
                FirstLoad = true
            };
        }

        public void DestroyRoomInstance(int userId)
            => roomInstanceProvider.DestroyService(userId);
    }
}