using NextHave.BL.Clients;
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

        Task<RoomModel?> GetRoomModel(string modelName, int? roomId = null);

        Task<IRoomInstance?> GetRoomInstance(int roomId);

        Task DisposeRoom(int roomId);
        
        Task SaveRoom(Room room, Client client, NavigatorCategory category);
    }
}