namespace NextHave.API.Conf
{
    class DolphinConfigurationSource(string environmentVariableName) : IConfigurationSource
    {
        IConfigurationProvider IConfigurationSource.Build(IConfigurationBuilder builder)
            => new DolphinConfigurationProvider(environmentVariableName);
    }
}