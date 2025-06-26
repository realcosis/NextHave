namespace NextHave.BL.Utils
{
    public static class RotationCalculatorUtility
    {
        private static readonly int[,] DirectionLookup = InitializeDirectionLookup();

        private static int[,] InitializeDirectionLookup()
        {
            var lookup = new int[3, 3];

            lookup[0, 0] = 7;
            lookup[0, 1] = 6;
            lookup[0, 2] = 5;
            lookup[1, 0] = 0;
            lookup[1, 1] = 0;
            lookup[1, 2] = 4;
            lookup[2, 0] = 1;
            lookup[2, 1] = 2;
            lookup[2, 2] = 3;

            return lookup;
        }

        public static int Calculate(int startX, int startY, int endX, int endY, bool moonwalk = false)
        {
            var dx = Math.Sign(endX - startX) + 1;
            var dy = Math.Sign(endY - startY) + 1;

            var rotation = DirectionLookup[dx, dy];

            return moonwalk ? ((rotation <= 3) ? (rotation + 4) : (rotation - 4)) : rotation;
        }
    }
}