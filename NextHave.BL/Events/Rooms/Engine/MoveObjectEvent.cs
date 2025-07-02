namespace NextHave.BL.Events.Rooms.Engine
{
    public class MoveObjectEvent : RoomEvent
    {
        public int ItemId { get; set; }

        public int NewX { get; set; }

        public int NewY { get; set; }

        public int Rotation { get; set; }
    }
}