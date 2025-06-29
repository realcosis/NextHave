namespace NextHave.BL.Models
{
    public class Point(int x, int y) : IEquatable<Point>
    {
        public int GetX
            => x;

        public int GetY
            => y;

        public static Point operator +(Point left, Point right)
            => new(left.GetX + right.GetX, left.GetY + right.GetY);

        public static bool operator !=(Point? left, Point? right)
            => !(left == right);

        public static bool operator ==(Point? left, Point? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public override int GetHashCode()
            => HashCode.Combine(GetX, GetY);

        public override bool Equals(object? obj)
            => obj is Point point && Equals(point);

        public bool Equals(Point? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return GetX == other.GetX && GetY == other.GetY;
        }

        public int GetDistance(Point other)
        {
            var x = GetX - other.GetX;
            var y = GetY - other.GetY;
            return x * x + y * y;
        }

        public override string ToString()
            => $"({GetX}, {GetY})";
    }
}