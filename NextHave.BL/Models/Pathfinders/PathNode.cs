namespace NextHave.BL.Models.Pathfinders
{
    public class PathNode(Point point) : IComparable<PathNode>
    {
        public Point Position { get; set; } = point;

        public PathNode? Next { get; set; }

        public int Cost { get; set; } = int.MaxValue;

        public bool InOpen { get; set; } = false;

        public bool InClosed { get; set; } = false;

        public bool Equals(PathNode? breadcrumb)
            => breadcrumb != default && breadcrumb.Position.Equals(Position);

        public override int GetHashCode()
            => Position.GetHashCode();

        public int CompareTo(PathNode? other)
            => Cost.CompareTo(other?.Cost);
    }
}