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

        Task<User?> Login(UserLoginWrite userLogin);

        Task<User> Register(UserRegistrationWrite userRegistration, string? registrationIp);

        Task<User?> GetFromToken(int userId);

        Task<string?> GetAndSetAuthToken(int userId);
    }
}