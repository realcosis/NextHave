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
using Dolphin.Core.Exceptions;
using NextHave.Localizations;

namespace Dolphin.HabboHotel.Users.Models
{
    [Service(ServiceLifetime.Scoped)]
    class UsersService(IDbContextFactory<MySQLDbContext> mysqlDbContextFactory, IEventsManager eventsManager,
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

        async Task<User?> IUsersService.LoadHabbo(string authTicket)
        {
            try
            {
                await using var mysqlDbContext = await mysqlDbContextFactory.CreateDbContextAsync();
                var userTicket = await mysqlDbContext
                                        .UserTickets
                                            .Include(ut => ut.User)
                                            .FirstOrDefaultAsync(ut => ut.Ticket == authTicket) ?? throw new DolphinException(Errors.UserTicketNotFound);

                var user = await mysqlDbContext
                                    .Users
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(u => u.Id == userTicket.UserId) ?? throw new DolphinException(Errors.UserNotFound);

                userTicket.UsedAt = DateTime.Now;

                mysqlDbContext.UserTickets.Update(userTicket);
                await mysqlDbContext.SaveChangesAsync();

                return new User
                {
                    
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading Habbo with auth ticket: {AuthTicket}", authTicket);
                return default;
            }
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