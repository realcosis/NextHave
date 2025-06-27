namespace NextHave.BL.Contexts
{
    public class TileReservationContext
    {
        public int VirtualId { get; set; }

        public DateTime ReservationTime { get; set; }

        public bool IsConfirmed { get; set; }
    }
}