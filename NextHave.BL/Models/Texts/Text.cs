namespace NextHave.BL.Models.Texts
{
    public class Text
    {
        public string? Key { get; set; }

        public string? Value { get; set; }

        public string? Description { get; set; }

        public static Text Create(string? key, string? value, string? description)
            => new()
            {
                Key = key,
                Value = value,
                Description = description,
            };
    }
}