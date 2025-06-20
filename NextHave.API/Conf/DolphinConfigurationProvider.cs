namespace NextHave.API.Conf
{
    class DolphinConfigurationProvider(string environmentVariableName) : ConfigurationProvider
    {
        public override void Load()
        {
            Data = environmentVariableName.ReadConfigurationFile()?.ToFlatDictionary() ?? new Dictionary<string, string?>();
        }
    }
}