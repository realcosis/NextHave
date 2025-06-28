using NextHave.BL.Events.Rooms;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Components;

namespace NextHave.BL.Services.Rooms.Instances
{
    class RoomInstance(IEnumerable<IRoomComponent> roomComponents, RoomEventsFactory roomEventsFactory) : IRoomInstance
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

        public WorkRoomModel? RoomModel { get; set; }

        public RoomEventsService EventsService
            => roomEventsFactory.GetForRoom(Room!.Id);

        public async Task Init()
        {
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