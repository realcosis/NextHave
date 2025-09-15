namespace NextHave.BL.Models
{
    public class TryLoadReference<T> where T : class
    {
        public bool FirstLoad { get; set; }

        public T? Reference { get; set; }
    }
}