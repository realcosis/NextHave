using NextHave.BL.Models.Rooms;

namespace NextHave.BL.Models.Navigators
{
    public class NavigatorPublicCategory
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public List<Room> Rooms { get; set; } = [];

        public int OrderNum { get; set; }

        public void AddRoom(Room room)
            => Rooms.Add(room);

        public void RemoveRoom(Room room)
            => Rooms.Remove(room);
    }
}