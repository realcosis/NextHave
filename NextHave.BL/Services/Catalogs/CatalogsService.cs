using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NextHave.BL.Services.Catalogs
{
    [Service(ServiceLifetime.Singleton)]
    class CatalogsService(ILogger<ICatalogsService> logger) : ICatalogsService, IStartableService
    {
        ICatalogsService Instance => this;

        async Task IStartableService.StartAsync()
        {
            try
            {
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogWarning("Exception during starting of CatalogsService: {ex}", ex);
            }
        }
    }
}