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

        public DbSet<RoomEmojiEntity> RoomEmojis { get; set; }

        public DbSet<ChatlogRoomEntity> ChatlogRooms { get; set; }

        public DbSet<ChatlogPrivateEntity> ChatlogPrivates { get; set; }

        public DbSet<ChatlogPrivateDetailEntity> ChatlogPrivateDetails { get; set; }

        public DbSet<UserTicketEntity> UserTickets { get; set; }

        public DbSet<NextHaveSettingEntity> NextHaveSettings { get; set; }

        public DbSet<NextHaveTextEntity> NextHaveTexts { get; set; }

        public DbSet<NavigatorPublicCategoryEntity> NavigatorPublicCategories { get; set; }

        public DbSet<NavigatorUserCategoryEntity> NavigatorUserCategories { get; set; }

        public DbSet<NavigatorPublicRoomEntity> NavigatorPublicRooms { get; set; }

        public DbSet<RoomModelEntity> RoomModels { get; set; }

        public DbSet<RoomModelCustomEntity> RoomModelCustoms { get; set; }

        public DbSet<RoomTonerEntity> RoomToners { get; set; }

        public DbSet<PermissionGroupEntity> PermissionGroups { get; set; }

        public DbSet<PermissionRightEntity> PermissionRights { get; set; }

        public DbSet<PermissionEntity> Permissions { get; set; }

        public DbSet<GroupEntity> Groups { get; set; }

        public DbSet<GroupElementEntity> GroupElements { get; set; }

        public DbSet<UserFavoriteEntity> UserFavorites { get; set; }

        public DbSet<MessengerFriendshipEntity> MessengerFriendships { get; set; }

        public DbSet<MessengerRequestEntity> MessengerRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PermissionEntity>().HasKey(e => new { e.GroupId, e.RightId });

            builder.Entity<GroupMembershipEntity>().HasKey(e => new { e.GroupId, e.UserId });

            builder.Entity<RoomTonerEntity>().HasKey(e => new { e.RoomId, e.ItemId });

            builder.Entity<UserFavoriteEntity>().HasKey(e => new { e.UserId, e.RoomId });

            builder.Entity<NextHaveSettingEntity>().HasKey(e => new { e.Key, e.Type });

            builder.Entity<MessengerFriendshipEntity>().HasKey(e => new { e.Sender, e.Receiver });

            builder.Entity<MessengerRequestEntity>().HasKey(e => new { e.Sender, e.Receiver });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }
    }
}