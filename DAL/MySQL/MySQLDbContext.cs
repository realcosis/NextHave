using Dolphin.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NextHave.DAL.MySQL.Entities;
using Dolphin.Core.Configurations.Models;

namespace NextHave.DAL.MySQL
{
    public class MySQLDbContext(IOptions<MySQLConfiguration> mysqlConfigurationOptionss, IOptions<PoolConfiguration> poolConfigurationOptionss) : MySQLDBContext(mysqlConfigurationOptionss, poolConfigurationOptionss)
    {
        public DbSet<ItemDefinitionEntity> ItemDefinitions { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<UserTicketEntity> UserTickets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserTicketEntity>().HasKey(e => new { e.UserId, e.Ticket });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder
                .UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
    }
}