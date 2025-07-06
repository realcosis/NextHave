using Dolphin.Core.Injection;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Users.Instances;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Users.Factories
{
    [Service(ServiceLifetime.Scoped)]
    class UserFactory(IKeyedServicesProvider<IUserInstance> userInstanceProvider)
    {
        public IUserInstance? GetUserInstance(int userId)
        {
            if (userInstanceProvider.HasServiceRegstered(userId, out var userInstance))
                return userInstance;

            return default;
        }

        public IUserInstance GetUserInstance(int userId, User user)
        {
            if (userInstanceProvider.HasServiceRegstered(userId, out var userInstance))
                return userInstance!;

            userInstance = userInstanceProvider.GetRequiredKeyedService(userId);
            userInstance.User = user;

            return userInstance;
        }

        public void DestroyUserInstance(int userId)
            => userInstanceProvider.Disable(userId);
    }
}