using Dolphin.Core.Exceptions;
using Dolphin.Core.Injection;
using Dolphin.Core.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Events.Users.Session;
using NextHave.BL.Extensions;
using NextHave.BL.Localizations;
using NextHave.BL.Mappers;
using NextHave.BL.Models.Groups;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Permissions;
using NextHave.BL.Services.Settings;
using NextHave.BL.Services.Users.Factories;
using NextHave.BL.Services.Users.Instances;
using NextHave.DAL.Mongo;
using NextHave.DAL.MySQL;
using NextHave.DAL.MySQL.Entities;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Users
{
    [Service(ServiceLifetime.Singleton)]
    class UsersService(IServiceScopeFactory serviceScopeFactory, ILogger<IUsersService> logger, ISettingsService settingsService) : IUsersService
    {
        IUsersService Instance => this;

        ConcurrentDictionary<int, IUserInstance> IUsersService.Users { get; } = [];

        async Task<User?> IUsersService.GetHabbo(int userId)
        {
            var mysqlDbContext = await serviceScopeFactory.GetRequiredService<MySQLDbContext>();

            var user = await mysqlDbContext
                                .Users
                                    .Where(u => u.Id == userId)
                                    .FirstOrDefaultAsync() ?? throw new DolphinException(Errors.UserNotFound);

            return user.MapResult();
        }

        async Task<IUserInstance?> IUsersService.LoadHabbo(string authTicket, int time)
        {
            try
            {
                var date = DateTime.Now.AddMilliseconds(time);

                var mysqlDbContext = await serviceScopeFactory.GetRequiredService<MySQLDbContext>();

                var mongoDbContext = await serviceScopeFactory.GetRequiredService<MongoDbContext>();

                var permissionsService = await serviceScopeFactory.GetRequiredService<IPermissionsService>();

                var userFactory = await serviceScopeFactory.GetRequiredService<UserFactory>();

                var userTicket = await mysqlDbContext
                                        .UserTickets
                                            .Include(ut => ut.User)
                                                .ThenInclude(u => u!.Favorites)
                                            .Include(ut => ut.User)
                                                .ThenInclude(u => u!.GroupMemberships)
                                                    .ThenInclude(u => u.Group)
                                            .FirstOrDefaultAsync(ut => ut.Ticket == authTicket) ?? throw new DolphinException(Errors.UserTicketNotFound);

                var user = userTicket.User ?? throw new DolphinException(Errors.UserNotFound);

                user.LastOnline = date;

                mysqlDbContext.Users.Update(user);
                await mysqlDbContext.SaveChangesAsync();

                var rooms = await mongoDbContext.Rooms.Where(r => r.Author!.AuthorId == user.Id).AsNoTracking().ToListAsync();

                var groupIds = rooms.Where(r => r.Group != default).Select(r => r.Group!.GroupId).Distinct().ToList();
                var groups = await mysqlDbContext.Groups.AsNoTracking().Where(g => groupIds.Contains(g.Id)).Select(g => g.Map()).ToListAsync();

                var resultRooms = rooms.Select(r => r.Map(groups.FirstOrDefault(g => g.RoomId == r.EntityId), 0)).ToList();

                var favoriteRooms = new List<Room>();
                var roomIds = user.Favorites.Select(f => f.RoomId);

                groupIds = [.. rooms.Where(r => r.Group != default).Select(r => r.Group!.GroupId).Distinct()];
                groups = await mysqlDbContext.Groups.AsNoTracking().Where(g => roomIds.Contains(g.RoomId)).Select(g => g.Map()).ToListAsync();

                var dbFavoritesRooms = await mongoDbContext.Rooms.Where(r => roomIds.Contains(r.EntityId)).AsNoTracking().ToListAsync();
                foreach (var room in dbFavoritesRooms)
                {
                    var group = default(Group?);
                    if (room.Group != default)
                        group = groups.FirstOrDefault(g => g.Id == room.Group.GroupId);
                    favoriteRooms.Add(room.Map(group));
                }

                groups = [.. user.GroupMemberships.Select(gm => gm.Group).Select(g => g!.Map())];
                var groupRoomIds = groups.Select(g => g.RoomId).Distinct().ToList();
                var groupRooms = await mongoDbContext.Rooms.Where(r => groupRoomIds.Contains(r.EntityId)).AsNoTracking().ToListAsync();
                foreach (var group in groups)
                    group.Room = groupRooms.FirstOrDefault(gr => gr.EntityId == group.RoomId)?.Map(group);

                var result = user.MapResult();
                result.Rooms = resultRooms;
                result.FavoriteRooms = favoriteRooms;
                result.Groups = groups;

                var userInstance = userFactory.GetUserInstance(result.Id, result);

                if (permissionsService.Groups.TryGetValue(user.Rank!.Value, out var permissionGroup))
                    userInstance.Permission = permissionGroup;

                await userInstance.Init();

                Instance.Users.TryAdd(userInstance.User!.Id, userInstance);

                await userInstance.EventsService.SubscribeAsync<UserDisconnectedEvent>(userInstance, OnUserDisconnected);

                return userInstance;
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

            var mysqlDbContext = await serviceScopeFactory.GetRequiredService<MySQLDbContext>();
            var user = await mysqlDbContext
                                    .Users
                                        .FirstOrDefaultAsync(u => u.Username!.ToLower() == userLogin!.Username!.ToLower() || u.Mail! == userLogin!.Username!.ToLower()) ?? throw new DolphinException(Errors.UserNotFound);

            if (!userLogin!.Password!.VerifyPassword(user.Password!))
                throw new DolphinException(Errors.InvalidPassword);

            return user.MapResult();
        }

        async Task<User> IUsersService.Register(UserRegistrationWrite userRegistration, string? registrationIp)
        {
            userRegistration?.Validate();

            var mysqlDbContext = await serviceScopeFactory.GetRequiredService<MySQLDbContext>();

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
            var mysqlDbContext = await serviceScopeFactory.GetRequiredService<MySQLDbContext>();

            var user = await mysqlDbContext
                                    .Users
                                        .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new DolphinException(Errors.UserNotFound);

            return user.MapResult();
        }

        async Task<string?> IUsersService.GetAndSetAuthToken(int userId)
        {
            var mysqlDbContext = await serviceScopeFactory.GetRequiredService<MySQLDbContext>();

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

        #region private methods

        async Task OnUserDisconnected(UserDisconnectedEvent @event)
        {
            if (Instance.Users.TryGetValue(@event.UserId, out var userInstance) && userInstance.User != default)
            {
                await userInstance.EventsService.UnsubscribeAsync<UserDisconnectedEvent>(userInstance, OnUserDisconnected);

                await userInstance.Dispose();

                (await serviceScopeFactory.GetRequiredService<UserFactory>()).DestroyUserInstance(@event.UserId);

                Instance.Users.TryRemove(@event.UserId, out _);
            }
        }

        #endregion
    }
}