using NextHave.BL.Enums;
using NextHave.BL.Utils;
using System.Collections.Concurrent;

namespace NextHave.BL.Models.Rooms
{
    public class WorkRoomModel
    {
        readonly RoomModel _baseModel;

        readonly Room _room;

        public int DoorX;

        public int DoorY;

        public double DoorZ;

        public int DoorOrientation;

        public string Heightmap;

        public ConcurrentDictionary<Point, TileStates> Tiles;

        public ConcurrentDictionary<Point, short> FloorHeights;

        public int MapSizeX;

        public int MapSizeY;

        public bool ClubOnly;

        public WorkRoomModel(Room room, RoomModel baseModel)
        {
            _baseModel = baseModel;
            _room = room;
            DoorX = _baseModel.DoorX;
            DoorY = _baseModel.DoorY;
            DoorZ = _baseModel.DoorZ;
            DoorOrientation = _baseModel.DoorOrientation;
            Heightmap = _baseModel.Heightmap.ToLower();
            MapSizeX = _baseModel.HeightmapArray[0].Length;
            MapSizeY = _baseModel.HeightmapArray.Length;
            ClubOnly = _baseModel.ClubOnly;
            Tiles = new ConcurrentDictionary<Point, TileStates>();
            FloorHeights = new ConcurrentDictionary<Point, short>();

            for (var y = 0; y < MapSizeY; y++)
            {
                for (var x = 0; x < MapSizeX; x++)
                {
                    if (x > _baseModel.MapSizeX - 1 || y > _baseModel.MapSizeY - 1)
                        Tiles.TryAdd(new Point(x, y), TileStates.BLOCKED);
                    else if (_baseModel.Tiles.TryGetValue(new Point(x, y), out var tileState))
                    {
                        Tiles.TryAdd(new Point(x, y), tileState);
                        if (_baseModel.FloorHeights.TryGetValue(new Point(x, y), out var floorHeight))
                            FloorHeights.TryAdd(new Point(x, y), floorHeight);

                    }
                }
            }
        }
    }
}