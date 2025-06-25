namespace NextHave.BL.Models
{
    public class Point(int x, int y)
    {
        public int GetX
            => x;

        public int GetY
            => y;

        public static bool operator !=(Point left, Point right)
            => left != right;

        public static bool operator ==(Point left, Point right)
            => left.Equals(right);

        public override int GetHashCode()
            => x ^ y;

        public override bool Equals(object? obj)
            => obj is not null && obj is Point point && point.GetX == GetX && point.GetY == GetY;
    }
}