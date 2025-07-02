namespace NextHave.BL.Models.Permissions
{
    public class Permission
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Badge { get; set; }

        public int? SecurityLevel { get; set; }

        public List<string> Rights { get; set; } = [];

        public bool HasRight(string right)
            => Rights.Contains(right);
    }
}