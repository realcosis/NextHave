using NextHave.DAL.MySQL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using NextHave.BL.Models.Users;
using NextHave.BL.Models.Badges;
using NextHave.BL.Models.Wardrobes;
using Dolphin.Core.Injection;
using Dolphin.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Localizations;
using NextHave.BL.Validations;
using NextHave.BL.Extensions;
using NextHave.BL.Mappers;
using NextHave.BL.Services.Settings;
using NextHave.DAL.MySQL.Entities;

namespace NextHave.BL.Services.Users
{
    [Service(ServiceLifetime.Scoped)]
    class UsersService(IDbContextFactory<MySQLDbContext> mysqlDbContextFactory, ILogger<IUsersService> logger, ISettingsService settingsService) : IUsersService
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
                                            .Include(ut => ut.User)
                                            .FirstOrDefaultAsync(ut => ut.Ticket == authTicket && !ut.UsedAt.HasValue) ?? throw new DolphinException(Errors.UserTicketNotFound);

                var user = userTicket.User ?? throw new DolphinException(Errors.UserNotFound);

                userTicket.UsedAt = date;
                user.LastOnline = date;

                mysqlDbContext.Users.Update(user);
                mysqlDbContext.UserTickets.Update(userTicket);
                await mysqlDbContext.SaveChangesAsync();

                return user.MapResult();
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

        async Task<User?> IUsersService.Login(UserLoginWrite userLogin)
        {
            userLogin?.Validate();

            await using var mysqlDbContext = await mysqlDbContextFactory.CreateDbContextAsync();
            var user = await mysqlDbContext
                                    .Users
                                        .FirstOrDefaultAsync(u => u.Username == userLogin!.Username || u.Mail == userLogin!.Username) ?? throw new DolphinException(Errors.UserNotFound);

            if (!userLogin!.Password!.VerifyPassword(user.Password!))
                throw new DolphinException(Errors.InvalidPassword);

            return user.MapResult();
        }

        async Task<User> IUsersService.Register(UserRegistrationWrite userRegistration, string? registrationIp)
        {
            userRegistration?.Validate();

            await using var mysqlDbContext = await mysqlDbContextFactory.CreateDbContextAsync();

            var hotelName = settingsService.GetSetting("hotel_name");
            var defaultLook = settingsService.GetSetting("default_look");

            if (await mysqlDbContext
                        .Users
                            .AsNoTracking()
                            .AnyAsync(u => u.Username!.Equals(userRegistration!.Username, StringComparison.CurrentCultureIgnoreCase)))
                throw new DolphinException(Errors.UsernameAlreadyTaked);

            if (await mysqlDbContext
                        .Users
                            .AsNoTracking()
                            .AnyAsync(u => u.Mail!.Equals(userRegistration!.Mail, StringComparison.CurrentCultureIgnoreCase)))
                throw new DolphinException(Errors.MailAlreadyTaked);

            var newUser = userRegistration!.MapRegistration(registrationIp, hotelName, defaultLook);

            await mysqlDbContext.Users.AddAsync(newUser);
            await mysqlDbContext.SaveChangesAsync();

            return newUser.MapResult();
        }

        async Task<User?> IUsersService.GetFromToken(int userId)
        {
            await using var mysqlDbContext = await mysqlDbContextFactory.CreateDbContextAsync();
            var user = await mysqlDbContext
                                    .Users
                                        .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new DolphinException(Errors.UserNotFound);
            return user.MapResult();
        }

        async Task<string?> IUsersService.GetAndSetAuthToken(int userId)
        {
            await using var mysqlDbContext = await mysqlDbContextFactory.CreateDbContextAsync();

            var newticket = Guid.NewGuid().ToString("N");
            var newUserTicket = new UserTicketEntity
            {
                UserId = userId,
                Ticket = newticket
            };

            await mysqlDbContext.UserTickets.AddAsync(newUserTicket);
            await mysqlDbContext.SaveChangesAsync();

            return newUserTicket.Ticket;
        }
    }
}