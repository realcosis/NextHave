using NextHave.Models.Users;
using NextHave.Models.Badges;
using NextHave.Models.Wardrobes;
using System.Collections.Concurrent;

namespace NextHave.Services.Users
{
    public interface IUsersService
    {
        ConcurrentDictionary<int, User> Users { get; }

        Task<User?> GetHabbo(int userId);

        Task<User?> LoadHabbo(string authTicket);

        Task GiveBadge(int userId, UserBadge badge);

        Task<bool> HasBadge(int userId, string badgeCode);

        Task RemoveBadge(int userId, UserBadge badge);

        Task UpsertSlot(int userId, UserWardrobe wardrobe);
    }
}