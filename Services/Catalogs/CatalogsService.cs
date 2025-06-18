using Dolphin.Core.Injection;

namespace NextHave.Services.Catalogs
{
    [Service(ServiceLifetime.Singleton)]
    class CatalogsService(ILogger<ICatalogsService> logger, IServiceProvider serviceProvider) : ICatalogsService, IStartableService
    {
        ICatalogsService Instance => this;

        async Task IStartableService.StartAsync()
        {
            try
            {

            }
            catch (Exception ex)
            {
                logger.LogWarning("Exception during starting of CatalogsService: {ex}", ex);
            }
        }
    }
}