namespace NextHave.BL.Models.Rooms
{
    public class RoomToner
    {
        public int RoomId { get; set; }

        public int ItemId { get; set; }

        public bool Enabled { get; set; }

        public int Hue { get; set; }

        public int Saturation { get; set; }

        public int Brightness { get; set; }
    }
}