using NextHave.DAL.MySQL;
using NextHave.DAL.Mongo;
using Dolphin.Core.Events;
using NextHave.Services.Users;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using NextHave.Models.Users;
using NextHave.Models.Badges;
using NextHave.Models.Wardrobes;
using Dolphin.Core.Injection;

namespace Dolphin.HabboHotel.Users.Models
{
    [Service(ServiceLifetime.Scoped)]
    class UsersService(IDbContextFactory<MongoDbContext> mysqlDbContextFactory, IEventsManager eventsManager,
                       IDbContextFactory<MongoDbContext> mongoDbcontextFactory, ILogger<IUsersService> logger) : IUsersService
    {
        ConcurrentDictionary<int, User> IUsersService.Users => throw new NotImplementedException();

        Task<User?> IUsersService.GetHabbo(int userId)
        {
            throw new NotImplementedException();
        }

        Task IUsersService.GiveBadge(int userId, UserBadge badge)
        {
            throw new NotImplementedException();
        }

        Task<bool> IUsersService.HasBadge(int userId, string badgeCode)
        {
            throw new NotImplementedException();
        }

        Task<User?> IUsersService.LoadHabbo(string authTicket)
        {
            throw new NotImplementedException();
        }

        Task IUsersService.RemoveBadge(int userId, UserBadge badge)
        {
            throw new NotImplementedException();
        }

        Task IUsersService.UpsertSlot(int userId, UserWardrobe wardrobe)
        {
            throw new NotImplementedException();
        }
    }
}