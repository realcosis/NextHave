using Dolphin.Core.Database;
using NextHave.DAL.Mongo.Entities;
using Microsoft.EntityFrameworkCore;

namespace NextHave.DAL.Mongo
{
    public class MongoDbContext(DbContextOptions<MongoDbContext> options) : MongoDBContext<MongoDbContext>(options)
    {
        public DbSet<CatalogPageEntity> CatalogPages { get; set; }

        public DbSet<RoomEntity> Rooms { get; set; }

        public DbSet<RoomItemEntity> RoomItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
            => base.OnModelCreating(builder);
    }
}