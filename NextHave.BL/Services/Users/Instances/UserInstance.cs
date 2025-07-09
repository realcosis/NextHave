using NextHave.BL.Clients;
using NextHave.BL.Models.Permissions;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Rooms;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Services.Users.Components;

namespace NextHave.BL.Services.Users.Instances
{
    class UserInstance(IEnumerable<IUserComponent> userComponents, UserEventsFactory userEventsFactory, RoomFactory roomFactory) : IUserInstance
    {
        IUserInstance Instance => this;

        User? IUserInstance.User { get; set; }

        UserEventsService IUserInstance.EventsService => userEventsFactory.Get(Instance.User!.Id);

        Client? IUserInstance.Client { get; set; }

        int? IUserInstance.CurrentRoomId { get; set; }

        IRoomInstance? IUserInstance.CurrentRoomInstance => Instance.CurrentRoomId.HasValue ? roomFactory.GetRoomInstance(Instance.CurrentRoomId!.Value) : default;

        Permission? IUserInstance.Permission { get; set; }

        bool IUserInstance.IsMuted { get; set; }

        DateTime? IUserInstance.MutedUntil { get; set; }

        async Task IUserInstance.Init()
        {
            foreach (var userComponent in userComponents)
                await userComponent.Init(this);
        }

        async Task IUserInstance.Dispose()
        {
            foreach (var userComponent in userComponents)
                await userComponent.Dispose();
            Instance.User = default;
            Instance.Client = default;
            Instance.CurrentRoomId = default;
            Instance.Permission = default;
        }
    }
}