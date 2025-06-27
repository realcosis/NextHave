using NextHave.BL.Models;

namespace NextHave.BL.Context
{
    public class UserMovementContext
    {
        public Point? GoalPoint { get; set; }

        public Point? NextPoint { get; set; }

        public DateTime RequestTime { get; set; }

        public bool IsProcessing { get; set; }

        public bool HasPendingStep { get; set; }
    }
}