using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Services.Rooms.Components
{
    public interface IRoomComponent
    {
        Task Init(IRoomInstance roomInstance);
    }
}