using NextHave.DAL.Enums;

namespace NextHave.BL.Models.Settings
{
    public class Setting
    {
        public string? Key { get; set; }

        public string? Value { get; set; }

        public SettingTypes? Type { get; set; }

        public string? Description { get; set; }

        public static Setting Create(string? key, string? value, SettingTypes? type, string? description)
            => new()
            {
                Key = key,
                Value = value,
                Type = type,
                Description = description,
            };
    }
}