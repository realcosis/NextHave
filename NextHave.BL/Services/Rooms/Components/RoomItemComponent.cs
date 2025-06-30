using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Events.Rooms;
using NextHave.BL.Events.Rooms.Items;
using NextHave.BL.Events.Rooms.Session;
using NextHave.BL.Mappers;
using NextHave.BL.Messages.Output.Rooms.Engine;
using NextHave.BL.Models.Items;
using NextHave.BL.Services.Items;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.DAL.Enums;
using NextHave.DAL.Mongo;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Rooms.Components
{
    [Service(ServiceLifetime.Scoped)]
    class RoomItemComponent(MongoDbContext dbContext, IServiceScopeFactory serviceScopeFactory) : IRoomComponent
    {
        IRoomInstance? _roomInstance;

        readonly ConcurrentDictionary<int, RoomItem> items = [];

        async Task IRoomComponent.Init(IRoomInstance roomInstance)
        {
            _roomInstance = roomInstance;

            await _roomInstance.EventsService.SubscribeAsync<RoomTickEvent>(_roomInstance, OnRoomTick);

            await _roomInstance.EventsService.SubscribeAsync<LoadRoomItemsEvent>(_roomInstance, OnLoadRoomItems);

            await _roomInstance.EventsService.SubscribeAsync<SendItemsToNewUserEvent>(_roomInstance, OnSendItemsToNewUser);
        }

        async Task OnRoomTick(RoomTickEvent @event)
        {
            await Task.CompletedTask;
        }

        async Task OnLoadRoomItems(LoadRoomItemsEvent @event)
        {
            if (_roomInstance?.Room == default || !items.IsEmpty)
                return;

            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var itemsService = scope.ServiceProvider.GetRequiredService<IItemsService>();

            var roomItems = await dbContext.RoomItems.AsNoTracking().Where(i => i.RoomId == @event.RoomId).ToListAsync();
            
            Parallel.ForEach(roomItems, new()
            {
                CancellationToken = new(),
                MaxDegreeOfParallelism = Environment.ProcessorCount
            }, item =>
            {
                if (itemsService.TryGetItemDefinition(item.BaseId!.Value, out var itemDefinition))
                    items.TryAdd(item.EntityId!.Value, item.Map(itemDefinition!, _roomInstance, 0));
            });
        }

        async Task OnSendItemsToNewUser(SendItemsToNewUserEvent @event)
        {
            if (_roomInstance?.Room == default)
                return;

            await _roomInstance.EventsService.DispatchAsync<SendRoomPacketEvent>(new()
            {
                RoomId = @event.RoomId,
                Composer = new ObjectsMessageComposer(_roomInstance.Room.OwnerId!.Value, _roomInstance.Room.Owner!, [.. items.Values.Where(i => i.Base != default && i.Base.Type == ItemTypes.Floor)])
            });
        }
    }
}