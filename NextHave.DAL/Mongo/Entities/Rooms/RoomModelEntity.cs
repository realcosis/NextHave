namespace NextHave.DAL.Mongo.Entities.Rooms
{
    public class RoomModelEntity
    {
        public string? ModelId { get; set; }

        public string? Heightmap { get; set; }

        public int DoorX { get; set; }

        public int DoorY { get; set; }

        public double DoorZ { get; set; }

        public int DoorOrientation { get; set; }
    }
}