using MongoDB.Driver;
using NextHave.BL.Enums;
using NextHave.BL.Messages;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Utils;
using System.Collections.Concurrent;
using System.Text;

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

        public IRoomInstance? RoomInstance { get; set; }

        public RoomModel(int doorX, int doorY, double doorZ, int doorOrientation, string heightmap)
        {
            DoorX = doorX;
            DoorY = doorY;
            DoorZ = doorZ;
            DoorOrientation = doorOrientation;
            Heightmap = heightmap.ToLower();
            HeightmapArray = Heightmap.Split(Convert.ToChar(13));
            MapSizeX = HeightmapArray[0].Length;
            MapSizeY = HeightmapArray.Length;
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

        public void SerializeHeightmap(ServerMessage serverMessage)
        {
            var stringBuilder = new StringBuilder();
            serverMessage.AddBoolean(true);
            serverMessage.AddInt32(RoomInstance!.Room!.WallHeight ?? 6);
            for (var y = 0; y < MapSizeY; y++)
            {
                for (var x = 0; x < MapSizeX; x++)
                {
                    if (x == DoorX && y == DoorY)
                        stringBuilder.Append(DoorZ);
                    else if (Tiles.TryGetValue(new Point(x, y), out var state) && state.Equals(TileStates.BLOCKED))
                        stringBuilder.Append('x');
                    else if (FloorHeights.TryGetValue(new Point(x, y), out var symbol))
                        stringBuilder.Append(symbol.ToString().ParseInput());
                }
                stringBuilder.Append('\r');
            }
            serverMessage.AddString(stringBuilder.ToString());
        }

        public void SerializeHeight(ServerMessage serverMessage)
        {
            var bytes = Encoding.ASCII.GetBytes("ÿÿ");

            serverMessage.AddInt32(MapSizeX);
            serverMessage.AddInt32(MapSizeX * MapSizeY);
            for (int y = 0; y < MapSizeY; y++)
            {
                for (int x = 0; x < MapSizeX; x++)
                {
                    if (Tiles.TryGetValue(new Point(x, y), out var state) && state.Equals(TileStates.BLOCKED))
                        serverMessage.AddBytes(bytes);
                    else if (x == DoorX && y == DoorY)
                    {
                        serverMessage.AddByte((byte)DoorZ);
                        serverMessage.AddByte(0);
                    }
                    else
                    {
                        var data = (byte)FloorHeights.FirstOrDefault(fh => fh.Key.GetX == x && fh.Key.GetY == y).Value;
                        serverMessage.AddByte(data);
                        serverMessage.AddByte(0);
                    }
                }
            }
        }

        public bool OutOfBounds(int x, int y)
            => x >= MapSizeX || y >= MapSizeY;
    }
}