using NextHave.BL.Models;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Models.Pathfinders;
using NextHave.BL.Services.Rooms.Instances;
using System.Text;

namespace NextHave.BL.Services.Rooms.Pathfinders
{
    public class NextHavePathfinder
    {
        static readonly List<Point> DiagMovePoints =
        [
            new(-1, -1),
            new(0, -1),
            new(1, -1),
            new(1, 0),
            new(1, 1),
            new(0, 1),
            new(-1, 1),
            new(-1, 0)
        ];

        static readonly List<Point> NoDiagMovePoints =
        [
            new(0, -1),
            new(1, 0),
            new(0, 1),
            new(-1, 0)
        ];

        public static double GetDistance(int x1, int y1, int x2, int y2)
            => Math.Sqrt(checked(Math.Pow(x1 - x2, 2.0) + Math.Pow(y1 - y2, 2.0)));

        public static List<Point> GetPaths(WorkRoomModel roomModel, bool allowDiagonal, Point start, Point end)
        {
            var paths = new List<Point>();
            var node = FindPath(roomModel, allowDiagonal, start, end);

            if (node != default)
            {
                var tempPath = new List<Point>();
                var current = node;

                while (current != default)
                {
                    tempPath.Add(current.Position);
                    current = current.Next;
                }

                tempPath.Reverse();
                if (tempPath.Count > 1)
                    paths.AddRange(tempPath.Skip(1));
            }

            return paths;
        }

        public static Point? GetPath(WorkRoomModel roomModel, bool allowDiagonal, Point start, Point end)
        {
            var node = FindPath(roomModel, allowDiagonal, start, end);

            if (node != default)
            {
                var tempPath = new List<Point>();
                var current = node;

                while (current != default)
                {
                    tempPath.Add(current.Position);
                    current = current.Next;
                }

                if (tempPath.Count >= 2)
                    return tempPath[^2];
                else if (tempPath.Count == 1)
                    return end;
            }

            return default;
        }

        private static PathNode? FindPath(WorkRoomModel roomModel, bool allowDiagonal, Point start, Point end)
        {
            var heaps = new PriorityQueue<PathNode, int>();

            var grid = new PathNode[roomModel.MapSizeX, roomModel.MapSizeY];

            var startNode = new PathNode(start)
            {
                Cost = 0
            };

            var lastNode = new PathNode(end);

            grid[startNode.Position.GetX, startNode.Position.GetY] = startNode;

            heaps.Enqueue(startNode, startNode.Cost);

            var maxIteraction = allowDiagonal ? DiagMovePoints.Count : NoDiagMovePoints.Count;

            while (heaps.Count > 0)
            {
                var currentNode = heaps.Dequeue();
                currentNode.InClosed = true;

                for (var i = 0; i < maxIteraction; i++)
                {
                    var newPoint = currentNode.Position + (allowDiagonal ? DiagMovePoints[i] : NoDiagMovePoints[i]);
                    var lastStep = newPoint.GetX == end.GetX && newPoint.GetY == end.GetY;
                    if (roomModel.CanWalk(newPoint.GetX, newPoint.GetY, 0.0, lastStep, false))
                    {
                        var pathNode = grid[newPoint.GetX, newPoint.GetY];
                        if (pathNode == default)
                        {
                            pathNode = new PathNode(newPoint);
                            grid[newPoint.GetX, newPoint.GetY] = pathNode;
                        }

                        if (!pathNode.InClosed)
                        {
                            var cost = 0;

                            if (currentNode.Position.GetX != pathNode.Position.GetX)
                                cost++;

                            if (currentNode.Position.GetY != pathNode.Position.GetY)
                                cost++;

                            var tileCost = currentNode.Cost + cost + pathNode.Position.GetDistance(end);
                            if (tileCost < pathNode.Cost)
                            {
                                pathNode.Cost = tileCost;
                                pathNode.Next = currentNode;
                            }
                            if (!pathNode.InOpen)
                            {
                                if (pathNode.Equals(lastNode))
                                {
                                    pathNode.Next = currentNode;
                                    return pathNode;
                                }
                                pathNode.InOpen = true;
                                heaps.Enqueue(pathNode, pathNode.Cost);
                            }
                        }
                    }
                }
            }

            return default;
        }
    }
}