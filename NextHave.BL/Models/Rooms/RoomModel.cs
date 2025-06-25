using NextHave.BL.Enums;
using NextHave.BL.Utils;
using System.Collections.Concurrent;

namespace NextHave.BL.Models.Rooms
{
    public class RoomModel
    {
        public int DoorX;

        public int DoorY;

        public double DoorZ;

        public int DoorOrientation;

        public string Heightmap;

        public string[] HeightmapArray;

        public ConcurrentDictionary<Point, TileStates> Tiles;

        public ConcurrentDictionary<Point, short> FloorHeights;

        public int MapSizeX;

        public int MapSizeY;

        public bool ClubOnly;

        public RoomModel(int doorX, int doorY, double doorZ, int doorOrientation, string heightmap, bool clubOnly)
        {
            DoorX = doorX;
            DoorY = doorY;
            DoorZ = doorZ;
            DoorOrientation = doorOrientation;
            Heightmap = heightmap.ToLower();
            HeightmapArray = Heightmap.Split(Convert.ToChar(13));
            MapSizeX = HeightmapArray[0].Length;
            MapSizeY = HeightmapArray.Length;
            ClubOnly = clubOnly;
            Tiles = new ConcurrentDictionary<Point, TileStates>();
            FloorHeights = new ConcurrentDictionary<Point, short>();

            for (var y = 0; y < MapSizeY; y++)
            {
                var row = HeightmapArray[y].Replace("\r", "").Replace("\n", "");
                var x = 0;
                for (var charIndex = 0; charIndex < row.Length; charIndex++)
                {
                    var symbol = row[charIndex];

                    if (symbol == 'x')
                        Tiles.TryAdd(new Point(x, y), TileStates.BLOCKED);
                    else
                    {
                        Tiles.TryAdd(new Point(x, y), TileStates.OPEN);
                        FloorHeights.TryAdd(new Point(x, y), symbol.ToString().ParseInput());
                    }
                    x++;
                }
            }
        }

        public bool OutOfBounds(int x, int y)
            => x >= MapSizeX || y >= MapSizeY;
    }
}