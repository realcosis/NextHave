using NextHave.BL.Models.Rooms;
using System.Collections.Concurrent;
using NextHave.BL.Models.Rooms.Navigators;
using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Services.Rooms
{
    public interface IRoomsService
    {
        ConcurrentDictionary<int, IRoomInstance> ActiveRooms { get; }

        Task<Room?> GetRoom(int roomId);

        Task<RoomModel?> GetRoomModel(string modelName, int roomId);

        Task<IRoomInstance?> GetRoomInstance(int roomId);
    }
}