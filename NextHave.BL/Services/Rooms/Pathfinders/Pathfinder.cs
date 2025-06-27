using NextHave.BL.Models;
using NextHave.BL.Models.Rooms;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;

namespace NextHave.BL.Services.Rooms.Pathfinders
{
    public class Pathfinder
    {
        Grid? _grid;

        PathFinder? _pathFinder;

        WorkRoomModel? _roomModel;

        public void Initialize(bool allowDiagonal, WorkRoomModel roomModel)
        {
            _pathFinder = new();

            _roomModel = roomModel;

            var gridSize = new GridSize(_roomModel.MapSizeX, _roomModel.MapSizeY);

            var cellSize = new Size(Distance.FromMeters(1), Distance.FromMeters(1));

            var traversalVelocity = Velocity.FromMetersPerSecond(1);

            if (allowDiagonal)
                _grid = Grid.CreateGridWithLateralAndDiagonalConnections(gridSize, cellSize, traversalVelocity);
            else
                _grid = Grid.CreateGridWithLateralConnections(gridSize, cellSize, traversalVelocity);

            for (int x = 0; x < _roomModel.MapSizeX; x++)
            {
                for (int y = 0; y < _roomModel.MapSizeY; y++)
                {
                    if (!_roomModel.CanWalk(x, y))
                    {
                        var gridPosition = new GridPosition(x, y);
                        _grid.DisconnectNode(gridPosition);
                        _grid.RemoveDiagonalConnectionsIntersectingWithNode(gridPosition);
                    }
                }
            }
        }

        public List<Point> FindPath(int startX, int startY, int endX, int endY, bool allowDiagonal = true)
        {
            if (_roomModel == null || _grid == null || _pathFinder == default)
                return [];

            if (!_roomModel.ValidTile(startX, startY) || !_roomModel.ValidTile(endX, endY))
                return [];

            if (startX == endX && startY == endY)
                return [];

            try
            {
                var startPosition = new GridPosition(startX, startY);
                var endPosition = new GridPosition(endX, endY);

                var gridToUse = _grid;
                if (!allowDiagonal)
                {
                    var gridSize = new GridSize(_roomModel.MapSizeX, _roomModel.MapSizeY);
                    var cellSize = new Size(Distance.FromMeters(1), Distance.FromMeters(1));
                    var traversalVelocity = Velocity.FromMetersPerSecond(1);

                    gridToUse = Grid.CreateGridWithLateralConnections(gridSize, cellSize, traversalVelocity);

                    for (int x = 0; x < _roomModel.MapSizeX; x++)
                    {
                        for (int y = 0; y < _roomModel.MapSizeY; y++)
                        {
                            if (!_roomModel.CanWalk(x, y))
                            {
                                gridToUse.DisconnectNode(new GridPosition(x, y));
                            }
                        }
                    }
                }

                var path = _pathFinder.FindPath(startPosition, endPosition, gridToUse);

                if (path.Type != PathType.Complete)
                    return [];

                var pathPoints = new List<Point>();
                foreach (var edge in path.Edges)
                {
                    var gridPos = edge.End;
                    pathPoints.Add(new Point((int)gridPos.Position.X, (int)gridPos.Position.Y));
                }

                return pathPoints;
            }
            catch
            {
                return [];
            }
        }

        public void UpdateGrid(bool allowDiagonal, WorkRoomModel roomModel)
            => Initialize(allowDiagonal, roomModel);

        public void SetCellBlocked(int x, int y, bool blocked, bool allowDiagonal, WorkRoomModel roomModel)
        {
            if (_grid != null && _roomModel != default && _roomModel.ValidTile(x, y))
            {
                var gridPosition = new GridPosition(x, y);

                if (blocked)
                {
                    _grid.DisconnectNode(gridPosition);
                    _grid.RemoveDiagonalConnectionsIntersectingWithNode(gridPosition);
                }
                else
                    UpdateGrid(allowDiagonal, roomModel);
            }
        }

        public List<Point> FindClosestPath(int startX, int startY, int endX, int endY, bool allowDiagonal = true)
        {
            if (_roomModel == null || _grid == null || _pathFinder == default)
                return [];

            var path = FindPath(startX, startY, endX, endY, allowDiagonal);

            if (path.Count > 0)
                return path;

            var startPosition = new GridPosition(startX, startY);
            var endPosition = new GridPosition(endX, endY);

            var pathResult = _pathFinder.FindPath(startPosition, endPosition, _grid);

            if (pathResult.Type == PathType.ClosestApproach && pathResult.Edges.Any())
                return [.. pathResult.Edges.Select(edge => new Point((int)edge.End.Position.X, (int)edge.End.Position.Y))];

            return [];
        }
    }
}