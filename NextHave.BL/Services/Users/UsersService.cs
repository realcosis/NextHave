using NextHave.DAL.MySQL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using NextHave.BL.Models.Users;
using NextHave.BL.Models.Badges;
using NextHave.BL.Models.Wardrobes;
using Dolphin.Core.Injection;
using Dolphin.Core.Exceptions;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Localizations;
using Dolphin.Core.Extensions;
using NextHave.BL.Validations;
using NextHave.BL.Extensions;

namespace NextHave.BL.Services.Users
{
    [Service(ServiceLifetime.Scoped)]
    class UsersService(IDbContextFactory<MySQLDbContext> mysqlDbContextFactory, ILogger<IUsersService> logger) : IUsersService
    {
        ConcurrentDictionary<int, User> IUsersService.Users => throw new NotImplementedException();

        Task IUsersService.GiveBadge(int userId, UserBadge badge)
        {
            throw new NotImplementedException();
        }

        Task<bool> IUsersService.HasBadge(int userId, string badgeCode)
        {
            throw new NotImplementedException();
        }

        #region Init

        Task<User?> IUsersService.GetHabbo(int userId)
        {
            throw new NotImplementedException();
        }

        async Task<User?> IUsersService.LoadHabbo(string authTicket, int time)
        {
            try
            {
                var date = DateTime.Now.AddMilliseconds(time);
                await using var mysqlDbContext = await mysqlDbContextFactory.CreateDbContextAsync();
                var userTicket = await mysqlDbContext
                                        .UserTickets
                                            .FirstOrDefaultAsync(ut => ut.Ticket == authTicket && !ut.UsedAt.HasValue) ?? throw new DolphinException(Errors.UserTicketNotFound);

                var user = await mysqlDbContext
                                    .Users
                                        .FirstOrDefaultAsync(u => u.Id == userTicket.UserId) ?? throw new DolphinException(Errors.UserNotFound);

                if (!Debugger.IsAttached)
                    userTicket.UsedAt = date;

                user.LastOnline = date;

                mysqlDbContext.Users.Update(user);
                mysqlDbContext.UserTickets.Update(userTicket);
                await mysqlDbContext.SaveChangesAsync();

                return new User
                {
                    Id = user.Id,
                    IsOnline = user.Online,
                    LastOnline = user.LastOnline,
                    Motto = user.Motto,
                    Rank = user.Rank!.Value,
                    Gender = user.Gender!.Value.GetDescription<EnumsDescriptions>(),
                    Look = user.Look,
                    Username = user.Username
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading Habbo with auth ticket: {AuthTicket}", authTicket);
                return default;
            }
        }

        #endregion

        Task IUsersService.RemoveBadge(int userId, UserBadge badge)
        {
            throw new NotImplementedException();
        }

        Task IUsersService.UpsertSlot(int userId, UserWardrobe wardrobe)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IUsersService.Login(UserLoginWrite userLogin)
        {
            userLogin?.Validate();

            await using var mysqlDbContext = await mysqlDbContextFactory.CreateDbContextAsync();
            var user = await mysqlDbContext
                                    .Users
                                        .FirstOrDefaultAsync(u => u.Username == userLogin!.Username || u.Mail == userLogin!.Username) ?? throw new DolphinException(Errors.UserNotFound);

            if (!userLogin!.Password!.VerifyPassword(user.Password!))
                throw new DolphinException(Errors.InvalidPassword);

            return true;
        }
    }
}