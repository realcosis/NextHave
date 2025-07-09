namespace NextHave.BL.Services.Texts
{
    public interface ITextsService
    {
        bool TryGetText(string key, string defaultValue, out string? value);

        string GetText(string key, string defaultValue);

        string FormatText(string key, string defaultValue, params object[] objects);
    }
}