using NextHave.BL.Models.Users;
using NextHave.BL.Models.Badges;
using NextHave.BL.Models.Wardrobes;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Users
{
    public interface IUsersService
    {
        ConcurrentDictionary<int, User> Users { get; }

        Task<User?> GetHabbo(int userId);

        Task<User?> LoadHabbo(string authTicket, int time);

        Task GiveBadge(int userId, UserBadge badge);

        Task<bool> HasBadge(int userId, string badgeCode);

        Task RemoveBadge(int userId, UserBadge badge);

        Task UpsertSlot(int userId, UserWardrobe wardrobe);

        Task<bool> Login(UserLoginWrite userLogin);
    }
}