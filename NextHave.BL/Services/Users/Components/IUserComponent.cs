using NextHave.BL.Services.Users.Instances;

namespace NextHave.BL.Services.Users.Components
{
    public interface IUserComponent
    {
        Task Dispose();

        Task Init(IUserInstance userInstance);
    }
}