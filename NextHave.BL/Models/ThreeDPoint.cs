namespace NextHave.BL.Models
{
    public class ThreeDPoint(int x, int y, double z) : Point(x, y), IEquatable<ThreeDPoint>
    {
        public double GetZ
            => z;

        public static bool operator !=(ThreeDPoint? left, ThreeDPoint? right)
            => !(left == right);

        public static bool operator ==(ThreeDPoint? left, ThreeDPoint? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.GetX == right.GetX && left.GetY == right.GetY && Math.Abs(left.GetZ - right.GetZ) < double.Epsilon;
        }

        public override int GetHashCode()
            => HashCode.Combine(GetX, GetY, GetZ);

        public override bool Equals(object? obj)
        {
            if (obj is ThreeDPoint threeDPoint)
                return Equals(threeDPoint);

            if (obj is Point point)
                return Equals(point);

            return false;
        }

        public bool Equals(ThreeDPoint? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return GetX == other.GetX && GetY == other.GetY && Math.Abs(GetZ - other.GetZ) < double.Epsilon;
        }

        public override string ToString()
            => $"({GetX}, {GetY}, {GetZ:F1})";
    }
}