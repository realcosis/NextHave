namespace NextHave.BL.Models
{
    public class TryGetReference<T> where T : class
    {
        public bool Exists { get; set; }

        public T? Reference { get; set; }
    }
}