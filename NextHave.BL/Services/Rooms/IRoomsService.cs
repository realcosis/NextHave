using NextHave.BL.Models;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Models.Rooms.Navigators;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Rooms
{
    public interface IRoomsService
    {
        ConcurrentDictionary<int, NavigatorCategory> NavigatorCategories { get; }

        List<Room> ActiveRooms { get; }

        Task<Room?> GetRoom(int roomId);

        Task<TryGetReference<Room>> TryGetRoom(int roomId);
    }
}