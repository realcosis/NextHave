using Dolphin.Core.Configurations.Models;
using Dolphin.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NextHave.DAL.MySQL.Entities;

namespace NextHave.DAL.MySQL
{
    public class MySQLDbContext(IOptions<MySQLConfiguration> mysqlConfigurationOptionss, IOptions<PoolConfiguration> poolConfigurationOptionss) : MySQLDBContext(mysqlConfigurationOptionss, poolConfigurationOptionss)
    {
        public DbSet<ItemDefinitionEntity> ItemDefinitions { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<UserTicketEntity> UserTickets { get; set; }

        public DbSet<NextHaveSettingEntity> NextHaveSettings { get; set; }

        public DbSet<NavigatorPublicCategoryEntity> NavigatorPublicCategories { get; set; }

        public DbSet<NavigatorUserCategoryEntity> NavigatorUserCategories { get; set; }

        public DbSet<NavigatorPublicRoomEntity> NavigatorPublicRooms { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<NextHaveSettingEntity>().HasKey(e => new { e.Key, e.Type });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }
    }
}