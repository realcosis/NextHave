namespace NextHave.BL.Services.Settings
{
    public interface ISettingsService
    {
        bool TryGetSetting(string key, out string? value);

        string? GetSetting(string key);
    }
}