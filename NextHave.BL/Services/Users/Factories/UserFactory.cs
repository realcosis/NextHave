using Dolphin.Core.Injection;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Users.Instances;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Users.Factories
{
    [Service(ServiceLifetime.Scoped)]
    class UserFactory(IKeyedServicesProvider<IUserInstance> userInstanceProvider)
    {
        public IUserInstance? GetUserInstance(int roomId)
        {
            if (userInstanceProvider.HasServiceRegstered(roomId, out var userInstance))
                return userInstance;

            return default;
        }

        public IUserInstance GetUserInstance(int roomId, User user)
        {
            if (userInstanceProvider.HasServiceRegstered(roomId, out var userInstance))
                return userInstance!;

            userInstance = userInstanceProvider.GetRequiredKeyedService(roomId);
            userInstance.User = user;

            return userInstance;
        }

        public void DestroyUserInstance(int userId)
            => userInstanceProvider.Disable(userId);
    }
}