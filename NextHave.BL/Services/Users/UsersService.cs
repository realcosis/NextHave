using NextHave.DAL.MySQL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using NextHave.BL.Models.Users;
using Dolphin.Core.Injection;
using Dolphin.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Localizations;
using Dolphin.Core.Validations;
using NextHave.BL.Extensions;
using NextHave.BL.Mappers;
using NextHave.BL.Services.Settings;
using NextHave.DAL.MySQL.Entities;
using Dolphin.Core.Events;

namespace NextHave.BL.Services.Users
{
    [Service(ServiceLifetime.Singleton)]
    class UsersService(IServiceScopeFactory serviceScopeFactory, ILogger<IUsersService> logger, ISettingsService settingsService, IEventsService eventsService) : IUsersService
    {
        IUsersService Instance => this;

        ConcurrentDictionary<int, User> IUsersService.Users { get; } = [];

        async Task<User?> IUsersService.GetHabbo(int userId)
        {
            using var scope = serviceScopeFactory.CreateAsyncScope();
            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();
            
            var user = await mysqlDbContext
                                .Users
                                    .Where(u => u.Id == userId)
                                    .FirstOrDefaultAsync() ?? throw new DolphinException(Errors.UserNotFound);

            return user.MapResult();
        }

        async Task<User?> IUsersService.LoadHabbo(string authTicket, int time)
        {
            try
            {
                var date = DateTime.Now.AddMilliseconds(time);

                using var scope = serviceScopeFactory.CreateAsyncScope();
                var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

                var userTicket = await mysqlDbContext
                                        .UserTickets
                                            .Include(ut => ut.User)
                                            .FirstOrDefaultAsync(ut => ut.Ticket == authTicket) ?? throw new DolphinException(Errors.UserTicketNotFound);

                var user = userTicket.User ?? throw new DolphinException(Errors.UserNotFound);

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

        async Task<User?> IUsersService.Login(UserLoginWrite userLogin)
        {
            userLogin?.Validate();

            using var scope = serviceScopeFactory.CreateAsyncScope();
            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();
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

            using var scope = serviceScopeFactory.CreateAsyncScope();
            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

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
            using var scope = serviceScopeFactory.CreateAsyncScope();
            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            var user = await mysqlDbContext
                                    .Users
                                        .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new DolphinException(Errors.UserNotFound);

            return user.MapResult();
        }

        async Task<string?> IUsersService.GetAndSetAuthToken(int userId)
        {
            using var scope = serviceScopeFactory.CreateAsyncScope();
            var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            var newticket = Guid.NewGuid().ToString("N");

            var ticket = await mysqlDbContext
                                        .UserTickets
                                            .FirstOrDefaultAsync(ut => ut.UserId == userId);

            if (ticket == default)
            {
                ticket = new UserTicketEntity()
                {
                    UserId = userId,
                    Ticket = newticket
                };
                await mysqlDbContext.UserTickets.AddAsync(ticket);
            }
            else
            {
                ticket.Ticket = newticket;
                mysqlDbContext.UserTickets.Update(ticket);
            }

            await mysqlDbContext.SaveChangesAsync();

            return ticket.Ticket;
        }
    }
}