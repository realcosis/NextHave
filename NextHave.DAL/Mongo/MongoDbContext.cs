using Dolphin.Core.Configurations.Models;
using Dolphin.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NextHave.DAL.Mongo.Entities;

namespace NextHave.DAL.Mongo
{
    public class MongoDbContext(IOptions<MongoConfiguration> mongoConfigurationOptions, IOptions<PoolConfiguration> poolConfigurationOptions) : MongoDBContext(mongoConfigurationOptions, poolConfigurationOptions)
    {
        public DbSet<CatalogPageEntity> CatalogPages { get; set; }

        public DbSet<RoomEntity> Rooms { get; set; }

        public DbSet<RoomItemEntity> RoomItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
            => base.OnConfiguring(builder);
    }
}