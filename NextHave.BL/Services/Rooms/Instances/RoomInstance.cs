using NextHave.BL.Events.Rooms;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Components;
using Microsoft.Extensions.DependencyInjection;
using NextHave.DAL.MySQL;
using Microsoft.EntityFrameworkCore;
using NextHave.BL.Mappers;

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
    }
}