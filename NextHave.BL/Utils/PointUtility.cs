using NextHave.BL.Models;

namespace NextHave.BL.Utils
{
    public static class PointUtility
    {
        public static Point ToPoint(this ThreeDPoint point)
            => new(point.GetX, point.GetY);

        public static ThreeDPoint ToThreeDPoint(this Point point, double z)
            => new(point.GetX, point.GetY, z);

        public static int Calculate(int startX, int startY, int endX, int endY, bool moonwalk)
        {
            int num = Calculate(startX, startY, endX, endY);
            if (!moonwalk)
            {
                return num;
            }
            return RotationIverse(num);
        }

        static int Calculate(int X1, int Y1, int X2, int Y2)
        {
            var result = 0;
            if (X1 > X2 && Y1 > Y2)
            {
                result = 7;
            }
            else if (X1 < X2 && Y1 < Y2)
            {
                result = 3;
            }
            else if (X1 > X2 && Y1 < Y2)
            {
                result = 5;
            }
            else if (X1 < X2 && Y1 > Y2)
            {
                result = 1;
            }
            else if (X1 > X2)
            {
                result = 6;
            }
            else if (X1 < X2)
            {
                result = 2;
            }
            else if (Y1 < Y2)
            {
                result = 4;
            }
            else if (Y1 > Y2)
            {
                result = 0;
            }
            return result;
        }

        static int RotationIverse(int rot)
        {
            rot = checked((rot <= 3) ? (rot + 4) : (rot - 4));
            return rot;
        }
    }
}