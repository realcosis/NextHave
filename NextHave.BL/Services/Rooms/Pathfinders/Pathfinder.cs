using NextHave.BL.Models;
using NextHave.BL.Models.Rooms;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System.ComponentModel.DataAnnotations;

namespace NextHave.BL.Services.Rooms.Pathfinders
{
    public class Pathfinder
    {
        Grid? _grid;

        PathFinder? _pathFinder;

        WorkRoomModel? _roomModel;

        public void Initialize(bool allowDiagonal, WorkRoomModel roomModel)
        {
            if (allowDiagonal)
                _grid = Grid.CreateGridWithLateralAndDiagonalConnections(new GridSize(roomModel.MapSizeX, roomModel.MapSizeY), new Size(Distance.FromMeters(1.0f), Distance.FromMeters(1.0f)), traversalVelocity: Velocity.FromKilometersPerHour(1));
            else
                _grid = Grid.CreateGridWithLateralConnections(new GridSize(roomModel.MapSizeX, roomModel.MapSizeY), new Size(Distance.FromMeters(1.0f), Distance.FromMeters(1.0f)), traversalVelocity: Velocity.FromKilometersPerHour(1));

            for (int x = 0; x < roomModel.MapSizeX; x++)
            {
                for (int y = 0; y < roomModel.MapSizeY; y++)
                {
                    if (!roomModel.CanWalk(x, y))
                    {
                        var pos = new GridPosition(x, y);
                        _grid.DisconnectNode(pos);
                    }
                }
            }

            _pathFinder = new();

            _roomModel = roomModel;
        }

        public List<Point> FindPath(int startX, int startY, int endX, int endY)
        {
            if (_roomModel == default || _pathFinder == default || _grid == default)
                return [];

            if (!_roomModel.ValidTile(startX, startY) || !_roomModel.ValidTile(endX, endY))
                return [];

            var path = _pathFinder.FindPath(new GridPosition(startX, startY), new GridPosition(endX, endY), _grid);

            return path.Edges.Any() ? [.. path.Edges.Select(e => e.End).Select(e => new Point((int)e.Position!.X, (int)e.Position!.Y))] : [];
        }
    }
}