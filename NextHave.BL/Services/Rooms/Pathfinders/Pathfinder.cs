using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using NextHave.BL.Models;
using Roy_T.AStar.Primitives;
using NextHave.BL.Models.Rooms;
using Path = Roy_T.AStar.Paths.Path;

namespace NextHave.BL.Services.Rooms.Pathfinders
{
    public class Pathfinder : IDisposable
    {
        Grid? _lateralGrid;

        Grid? _diagonalGrid;

        WorkRoomModel? _roomModel;

        readonly PathFinder _pathFinder;

        readonly Lock _lockObject = new();

        public Pathfinder()
            => _pathFinder = new PathFinder();

        public void Initialize(WorkRoomModel roomModel)
        {
            lock (_lockObject)
            {
                _roomModel = roomModel;

                _lateralGrid = CreateGrid(roomModel, false);
                _diagonalGrid = CreateGrid(roomModel, true);
            }
        }

        public List<Point> FindPath(int startX, int startY, int endX, int endY, bool allowDiagonal = true)
        {
            if (!IsInitialized() || !ValidateCoordinates(startX, startY, endX, endY))
                return [];

            if (startX == endX && startY == endY)
                return [];

            var path = CalculatePath(startX, startY, endX, endY, allowDiagonal);

            return path;
        }

        public List<Point> FindClosestPath(int startX, int startY, int endX, int endY, bool allowDiagonal = true)
        {
            if (!IsInitialized() || !ValidateCoordinates(startX, startY, endX, endY))
                return [];

            var completePath = FindPath(startX, startY, endX, endY, allowDiagonal);
            if (completePath.Count > 0)
                return completePath;

            return CalculateClosestApproach(startX, startY, endX, endY, allowDiagonal);
        }

        public void UpdateGrid(WorkRoomModel roomModel)
            => Initialize(roomModel);

        public void Dispose()
        {
            _lateralGrid = null;
            _diagonalGrid = null;
            _roomModel = null;
            GC.SuppressFinalize(this);
        }

        #region private methods

        static Grid CreateGrid(WorkRoomModel roomModel, bool allowDiagonal)
        {
            var gridSize = new GridSize(roomModel.MapSizeX, roomModel.MapSizeY);
            var cellSize = new Size(Distance.FromMeters(1), Distance.FromMeters(1));
            var traversalVelocity = Velocity.FromMetersPerSecond(1);

            var grid = allowDiagonal ? Grid.CreateGridWithLateralAndDiagonalConnections(gridSize, cellSize, traversalVelocity) : Grid.CreateGridWithLateralConnections(gridSize, cellSize, traversalVelocity);

            ApplyObstacles(grid, roomModel);

            return grid;
        }

        static void ApplyObstacles(Grid grid, WorkRoomModel roomModel)
        {
            for (int x = 0; x < roomModel.MapSizeX; x++)
            {
                for (int y = 0; y < roomModel.MapSizeY; y++)
                {
                    if (!roomModel.CanWalk(x, y))
                    {
                        var gridPosition = new GridPosition(x, y);
                        grid.DisconnectNode(gridPosition);
                        grid.RemoveDiagonalConnectionsIntersectingWithNode(gridPosition);
                    }
                }
            }
        }

        bool IsInitialized()
            => _roomModel != null && _lateralGrid != null && _diagonalGrid != null;

        bool ValidateCoordinates(int startX, int startY, int endX, int endY)
            => _roomModel!.ValidTile(startX, startY) && _roomModel.ValidTile(endX, endY);

        List<Point> CalculatePath(int startX, int startY, int endX, int endY, bool allowDiagonal)
        {
            try
            {
                var startPosition = new GridPosition(startX, startY);
                var endPosition = new GridPosition(endX, endY);
                var grid = allowDiagonal ? _diagonalGrid! : _lateralGrid!;

                var path = _pathFinder.FindPath(startPosition, endPosition, grid);

                if (path.Type != PathType.Complete)
                    return [];

                return ConvertPathToPoints(path);
            }
            catch
            {
                return [];
            }
        }

        List<Point> CalculateClosestApproach(int startX, int startY, int endX, int endY, bool allowDiagonal)
        {
            try
            {
                var startPosition = new GridPosition(startX, startY);
                var endPosition = new GridPosition(endX, endY);
                var grid = allowDiagonal ? _diagonalGrid! : _lateralGrid!;

                var path = _pathFinder.FindPath(startPosition, endPosition, grid);

                if (path.Type == PathType.ClosestApproach && path.Edges.Any())
                    return ConvertPathToPoints(path);

                return new List<Point>();
            }
            catch
            {
                return new List<Point>();
            }
        }

        static List<Point> ConvertPathToPoints(Path path)
        {
            var points = new List<Point>(path.Edges.Count);
            foreach (var edge in path.Edges)
            {
                points.Add(new Point((int)edge.End.Position.X, (int)edge.End.Position.Y));
            }
            return points;
        }

        void UpdateGridTile(Grid grid, GridPosition position, bool walkable, bool isDiagonal)
        {
            if (walkable)
            {
                if (isDiagonal)
                    _diagonalGrid = CreateGrid(_roomModel!, true);
                else
                    _lateralGrid = CreateGrid(_roomModel!, false);
            }
            else
            {
                grid.DisconnectNode(position);
                if (isDiagonal)
                    grid.RemoveDiagonalConnectionsIntersectingWithNode(position);
            }
        }

        #endregion
    }
}