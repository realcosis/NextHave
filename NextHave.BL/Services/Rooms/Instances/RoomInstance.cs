using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Events.Rooms;
using NextHave.BL.Mappers;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Services.Rooms.Components;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Users.Instances;
using NextHave.DAL.MySQL;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace NextHave.BL.Services.Rooms.Instances
{
    class RoomInstance(IEnumerable<IRoomComponent> roomComponents, RoomEventsFactory roomEventsFactory, IServiceScopeFactory serviceScopeFactory, ILogger<IRoomInstance> logger) : IRoomInstance
    {
        readonly ConcurrentDictionary<int, DateTime> mutedUsers = [];

        readonly List<IRoomComponent> Components = [];

        IRoomInstance Instance => this;

        bool hasRoom = false;
        Room? room;

        readonly Stopwatch Tick = new();

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
            var mysqlDbontext = await serviceScopeFactory.GetRequiredService<MySQLDbContext>();

            var dbToner = await mysqlDbontext.RoomToners.AsNoTracking().FirstOrDefaultAsync(t => t.RoomId == Instance.Room!.Id);

            if (dbToner != default)
                Instance.Toner = dbToner.Map();

            foreach (var roomComponent in roomComponents)
                await roomComponent.Init(this);

            Components.AddRange(roomComponents);

            Tick.Start();
        }

        async Task IRoomInstance.Tick()
        {
            if (Tick.ElapsedMilliseconds >= 500)
            {
                Tick.Restart();
                try
                {
                    await Instance.OnRoomTick();
                }
                catch (Exception ex)
                {
                    logger.LogError("Exception with OnRoomTick for room {RoomId}: {ex}", Instance.Room!.Id, ex);
                }
            }
        }

        async Task IRoomInstance.Dispose()
        {
            foreach (var roomComponent in Components)
                await roomComponent.Dispose();
            Components.Clear();
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