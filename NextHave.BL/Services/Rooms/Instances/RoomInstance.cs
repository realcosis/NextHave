using NextHave.BL.Events.Rooms;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Components;
using Microsoft.Extensions.DependencyInjection;
using NextHave.DAL.MySQL;
using Microsoft.EntityFrameworkCore;
using NextHave.BL.Mappers;
using NextHave.BL.Services.Users.Instances;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Rooms.Instances
{
    class RoomInstance(IEnumerable<IRoomComponent> roomComponents, RoomEventsFactory roomEventsFactory, IServiceScopeFactory serviceScopeFactory) : IRoomInstance
    {
        readonly ConcurrentDictionary<int, DateTime> mutedUsers = [];

        IRoomInstance Instance => this;

        bool hasRoom = false;
        Room? room;

        Room? IRoomInstance.Room
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

        bool IRoomInstance.RoomMuted { get; set; }

        RoomToner? IRoomInstance.Toner { get; set; }

        WorkRoomModel? IRoomInstance.RoomModel { get; set; }

        RoomEventsService IRoomInstance.EventsService => roomEventsFactory.Get(Instance.Room!.Id);

        async Task IRoomInstance.Init()
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mysqlDbontext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            var dbToner = await mysqlDbontext.RoomToners.AsNoTracking().FirstOrDefaultAsync(t => t.RoomId == Instance.Room!.Id);

            if (dbToner != default)
                Instance.Toner = dbToner.Map();

            foreach (var roomComponent in roomComponents)
                await roomComponent.Init(this);
        }

        async Task IRoomInstance.OnRoomTick()
        {
            if (Instance.Room == default)
                return;

            await Instance.EventsService.DispatchAsync<RoomTickEvent>(new()
            {
                RoomId = Instance.Room!.Id
            });
        }

        bool IRoomInstance.CheckRights(IUserInstance userInstance, bool isOwner)
        {
            if (userInstance == default || Instance.Room == default || userInstance.Permission == default)
                return false;

            if (Instance.Room.OwnerId.Equals(userInstance.User!.Id))
                return true;

            if (userInstance.Permission.HasRight("nexthave_administrator") || userInstance.Permission.HasRight("nexthave_all_rooms_owner"))
                return true;

            if (!isOwner)
            {
                if (userInstance.Permission.HasRight("nexthave_all_rooms_rights"))
                    return true;

                if (Instance.Room.Rights.Contains(userInstance.User!.Id))
                    return true;

                if (Instance.Room.AllowRightsOverride)
                    return true;

                if (Instance.Room.Group != default && Instance.Room.Group.Members.TryGetValue(userInstance.User!.Id, out var groupMember) && groupMember.Rank <= 1)
                    return true;
            }

            return false;
        }

        void IRoomInstance.MuteUser(int virtualId, DateTime until)
            => mutedUsers.AddOrUpdate(virtualId, until, (key, oldValue) => until > oldValue ? until : oldValue);

        bool IRoomInstance.CheckMute(int virtualId, IUserInstance userInstance)
        {
            if (mutedUsers.TryGetValue(virtualId, out var until))
            {
                if (until > DateTime.Now)
                    return true;
                mutedUsers.TryRemove(virtualId, out _);
            }

            if (userInstance.IsMuted || (Instance.RoomMuted && Instance.Room!.OwnerId.Equals(userInstance.User!.Id)))
                return true;

            return false;
        }
    }
}