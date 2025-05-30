using Dolphin.Core.Database;
using Dolphin.Core.Injection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Dolphin.Core.Configurations.Models;

namespace NextHave.DAL.Mongo
{
    [Service(ServiceLifetime.Scoped)]
    public class MongoDbContext(IOptions<MongoConfiguration> mongoConfigurationOptions, IOptions<PoolConfiguration> poolConfigurationOptions) : MongoDBContext(mongoConfigurationOptions, poolConfigurationOptions)
    {
        protected override void OnModelCreating(ModelBuilder builder)
           => base.OnModelCreating(builder);

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
            => base.OnConfiguring(builder);
    }
}