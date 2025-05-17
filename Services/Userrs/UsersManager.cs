using NextHave.DAL.MySQL;
using NextHave.DAL.Mongo;
using Dolphin.Core.Events;
using NextHave.Services.Users;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using NextHave.Models.Users;
using NextHave.Models.Badges;
using NextHave.Models.Wardrobes;

namespace Dolphin.HabboHotel.Users.Models
{
    class UsersManager(IDbContextFactory<MySQLDbContext> mysqlDbContextFactory, IEventsManager eventsManager,
                       IDbContextFactory<MongoDbContext> mongoDbcontextFactory, ILogger<IUsersManager> logger) : IUsersManager
    {
        ConcurrentDictionary<int, User> IUsersManager.Users => throw new NotImplementedException();

        Task<User?> IUsersManager.GetHabbo(int userId)
        {
            throw new NotImplementedException();
        }

        Task IUsersManager.GiveBadge(int userId, UserBadge badge)
        {
            throw new NotImplementedException();
        }

        Task<bool> IUsersManager.HasBadge(int userId, string badgeCode)
        {
            throw new NotImplementedException();
        }

        Task<User?> IUsersManager.LoadHabbo(string authTicket)
        {
            throw new NotImplementedException();
        }

        Task IUsersManager.RemoveBadge(int userId, UserBadge badge)
        {
            throw new NotImplementedException();
        }

        Task IUsersManager.UpsertSlot(int userId, UserWardrobe wardrobe)
        {
            throw new NotImplementedException();
        }
    }
}