using NextHave.BL.Services.Users.Instances;

namespace NextHave.BL.Services.Users.Components
{
    public interface IUserComponent
    {
        Task Init(IUserInstance userInstance);
    }
}