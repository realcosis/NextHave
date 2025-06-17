using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using NextHave.DAL.Mongo;

namespace NextHave.Services.Catalogs
{
    [Service(ServiceLifetime.Singleton)]
    class CatalogsService(ILogger<ICatalogsService> logger, IDbContextFactory<MongoDbContext> mongoDbContextFactory) : ICatalogsService, IStartableService
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