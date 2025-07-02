using NextHave.BL.Events.Rooms;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Components;
using Microsoft.Extensions.DependencyInjection;
using NextHave.DAL.MySQL;
using Microsoft.EntityFrameworkCore;
using NextHave.BL.Mappers;
using NextHave.BL.Models.Users;
using System.Globalization;

namespace NextHave.BL.Services.Rooms.Instances
{
    class RoomInstance(IEnumerable<IRoomComponent> roomComponents, RoomEventsFactory roomEventsFactory, IServiceScopeFactory serviceScopeFactory) : IRoomInstance
    {
        bool hasRoom = false;
        Room? room;

        public Room? Room
        {
            get => room;
            set
            {
                if (!hasRoom)
                {
                    room = value;
                    hasRoom = true;
                }
            }
        }

        public RoomToner? Toner { get; set; }

        public WorkRoomModel? RoomModel { get; set; }

        public RoomEventsService EventsService
            => roomEventsFactory.GetForRoom(Room!.Id);

        public async Task Init()
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mysqlDbontext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            var dbToner = await mysqlDbontext.RoomToners.AsNoTracking().FirstOrDefaultAsync(t => t.RoomId == Room!.Id);

            if (dbToner != default)
                Toner = dbToner.Map();

            foreach (var roomComponent in roomComponents)
                await roomComponent.Init(this);
        }

        public async Task OnRoomTick()
        {
            if (Room == default)
                return;

            await EventsService.DispatchAsync<RoomTickEvent>(new()
            {
                RoomId = Room!.Id
            });
        }

        bool IRoomInstance.CheckRights(User user, bool isOwner)
        {
            if (user == default || Room == default || user.Permission == default)
                return false;

            if (Room.OwnerId.Equals(user.Id))
                return true;

            if (user.Permission.HasRight("nexthave_administrator") || user.Permission.HasRight("nexthave_all_rooms_owner"))
                return true;

            if (!isOwner)
            {
                if (user.Permission.HasRight("nexthave_all_rooms_rights"))
                    return true;

                if (Room.Rights.Contains(user.Id))
                    return true;

                if (Room.AllowRightsOverride)
                    return true;

                if (Room.Group != default && Room.Group.Members.TryGetValue(user.Id, out var groupMember) && groupMember.Rank <= 1)
                    return true;
            }

            return false;
        }
    }
}