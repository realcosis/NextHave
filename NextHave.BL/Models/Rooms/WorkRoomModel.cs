using System.Text;
using NextHave.BL.Enums;
using NextHave.BL.Utils;
using NextHave.BL.Messages;
using System.Collections.Concurrent;
using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Models.Rooms
{
    public class WorkRoomModel
    {
        readonly RoomModel _baseModel;

        readonly IRoomInstance _roomInstance;

        public int DoorX;

        public int DoorY;

        public double DoorZ;

        public int DoorOrientation;

        public string Heightmap;

        public ConcurrentDictionary<Point, TileStates> Tiles;

        public ConcurrentDictionary<Point, short> FloorHeights;

        public ConcurrentDictionary<Point, bool> Walkables;

        readonly ConcurrentDictionary<Point, List<IRoomUserInstance>> RoomUsers;

        public int MapSizeX;

        public int MapSizeY;

        public WorkRoomModel(IRoomInstance roomInstance, RoomModel baseModel)
        {
            _baseModel = baseModel;
            _roomInstance = roomInstance;
            DoorX = _baseModel.DoorX;
            DoorY = _baseModel.DoorY;
            DoorZ = _baseModel.DoorZ;
            DoorOrientation = _baseModel.DoorOrientation;
            Heightmap = _baseModel.Heightmap.ToLower();
            MapSizeX = _baseModel.HeightmapArray[0].Length;
            MapSizeY = _baseModel.HeightmapArray.Length;
            Tiles = new ConcurrentDictionary<Point, TileStates>();
            FloorHeights = new ConcurrentDictionary<Point, short>();
            Walkables = new ConcurrentDictionary<Point, bool>();
            RoomUsers = new ConcurrentDictionary<Point, List<IRoomUserInstance>>();

            for (var y = 0; y < MapSizeY; y++)
            {
                for (var x = 0; x < MapSizeX; x++)
                {
                    if (x > _baseModel.MapSizeX - 1 || y > _baseModel.MapSizeY - 1)
                        Tiles.TryAdd(new Point(x, y), TileStates.Blocked);
                    else if (_baseModel.Tiles.TryGetValue(new Point(x, y), out var tileState))
                    {
                        Tiles.TryAdd(new Point(x, y), tileState);
                        if (_baseModel.FloorHeights.TryGetValue(new Point(x, y), out var floorHeight))
                            FloorHeights.TryAdd(new Point(x, y), floorHeight);
                    }
                    Walkables.TryAdd(new Point(x, y), true);
                }
            }
        }

        public void SerializeHeightmap(ServerMessage serverMessage)
        {
            var stringBuilder = new StringBuilder();
            serverMessage.AddBoolean(true);
            serverMessage.AddInt32(_roomInstance!.Room!.WallHeight ?? 6);
            for (var y = 0; y < MapSizeY; y++)
            {
                for (var x = 0; x < MapSizeX; x++)
                {
                    if (x == DoorX && y == DoorY)
                        stringBuilder.Append(DoorZ);
                    else if (Tiles.TryGetValue(new Point(x, y), out var state) && state.Equals(TileStates.Blocked))
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
                    if (Tiles.TryGetValue(new Point(x, y), out var state) && state.Equals(TileStates.Blocked))
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

        public void AddUser(Point point, IRoomUserInstance roomUserInstance)
        {
            if (RoomUsers.TryGetValue(point, out var users))
                users.Add(roomUserInstance);
            else
                RoomUsers.TryAdd(point, [roomUserInstance]);
        }

        public void RemoveUser(Point point, IRoomUserInstance roomUserInstance)
        {
            if (RoomUsers.TryGetValue(point, out var users))
                users.Remove(roomUserInstance);
        }

        public void SetWalkable(int x, int y, bool walkable)
        {
            if (ValidTile(x, y))
                Walkables.AddOrUpdate(new Point(x, y), walkable, (_, value) => walkable);
        }

        public void UpdateUser(Point oldCoord, Point newCoord, IRoomUserInstance roomUserInstance)
        {
            RemoveUser(oldCoord, roomUserInstance);
            AddUser(newCoord, roomUserInstance);
        }

        public bool CanWalk(int x, int y, double z = 0.0, bool @override = false)
        {
            if (!ValidTile(x, y))
                return false;

            if (!@override && OccupiedTile(x, y, z))
                return false;

            if (Walkables.TryGetValue(new Point(x, y), out var walkableState))
                return walkableState || @override;

            return true;
        }

        public List<IRoomUserInstance> GetRoomUsers(Point point)
            => RoomUsers.TryGetValue(point, out var users) ? users : [];

        public bool IsDoorTile(int x, int y)
            => x == DoorX && y == DoorY;

        public bool ValidTile(int x, int y)
            => x >= 0 && x < MapSizeX && y >= 0 && y < MapSizeY && Tiles.TryGetValue(new Point(x, y), out var state) && !state.Equals(TileStates.Blocked);

        #region private methods

        bool OccupiedTile(int x, int y, double z)
            => (!_roomInstance.Room?.AllowWalkthrough ?? true) && GetRoomUsers(new Point(x, y)).Any(p => p.Position!.GetZ == z);

        #endregion
    }
}