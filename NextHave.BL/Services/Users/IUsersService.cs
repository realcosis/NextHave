using NextHave.BL.Models.Users;
using System.Collections.Concurrent;
using NextHave.BL.Services.Users.Instances;

namespace NextHave.BL.Services.Users
{
    public interface IUsersService
    {
        ConcurrentDictionary<int, IUserInstance> Users { get; }

        Task<User?> GetHabbo(int userId);

        Task<IUserInstance?> LoadHabbo(string authTicket, int time);

        Task<User?> Login(UserLoginWrite userLogin);

        Task<User> Register(UserRegistrationWrite userRegistration, string? registrationIp);

        Task<User?> GetFromToken(int userId);

        Task<string?> GetAndSetAuthToken(int userId);
    }
}