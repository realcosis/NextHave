using Dolphin.Core.Database;
using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Dolphin.Core.Configurations.Models;
using NextHave.DAL.Mongo.Entities;

namespace NextHave.DAL.Mongo
{
    [Service(ServiceLifetime.Scoped)]
    public class MongoDbContext(IOptions<MongoConfiguration> mongoConfigurationOptions, IOptions<PoolConfiguration> poolConfigurationOptions) : MongoDBContext(mongoConfigurationOptions, poolConfigurationOptions)
    {
        public DbSet<CatalogPageEntity> CatalogPages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
            => base.OnConfiguring(builder);
    }
}