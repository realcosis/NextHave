using NextHave.BL.Models;
using NextHave.BL.Models.Items;

namespace NextHave.BL.Utils
{
    public static class ItemUtility
    {
        public static Dictionary<int, ThreeDPoint> GetItemTiles(this RoomItem roomItem)
        {
            if (roomItem.Base!.Length <= 0 || roomItem.Base!.Width <= 0)
                return [];

            var points = new Dictionary<int, ThreeDPoint>();
            var index = 0;

            var isHorizontal = roomItem.Rotation == 0 || roomItem.Rotation == 4;

            var effectiveLength = isHorizontal ? roomItem.Base!.Length : roomItem.Base!.Width;
            var effectiveWidth = isHorizontal ? roomItem.Base!.Width : roomItem.Base!.Length;

            for (var x = 0; x < effectiveLength; x++)
            {
                for (var y = 0; y < effectiveWidth; y++)
                {
                    if (x != 0 && y != 0)
                    {
                        int z = Math.Max(x, y);

                        int finalX, finalY;
                        if (isHorizontal)
                        {
                            finalX = roomItem.Point!.GetX + y;
                            finalY = roomItem.Point!.GetY + x;
                        }
                        else
                        {
                            finalX = roomItem.Point!.GetX + x;
                            finalY = roomItem.Point!.GetY + y;
                        }

                        points.Add(index++, new ThreeDPoint(finalX, finalY, z));
                    }
                }
            }

            return points;
        }
    }
}