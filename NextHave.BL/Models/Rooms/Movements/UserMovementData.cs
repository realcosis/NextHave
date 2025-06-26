namespace NextHave.BL.Models.Rooms.Movements
{
    public class UserMovementData
    {
        public List<Point> Points { get; set; } = [];

        public int CurrentPathIndex { get; set; } = 0;

        public Point? NextPoint { get; set; }

        public bool IsMoving { get; set; } = false;
    }
}