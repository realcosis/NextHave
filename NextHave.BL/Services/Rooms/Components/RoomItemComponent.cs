using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Events.Rooms;
using NextHave.BL.Events.Rooms.Engine;
using NextHave.BL.Events.Rooms.Items;
using NextHave.BL.Mappers;
using NextHave.BL.Messages.Output.Rooms.Engine;
using NextHave.BL.Models;
using NextHave.BL.Models.Items;
using NextHave.BL.Services.Items;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.DAL.Enums;
using NextHave.DAL.Mongo;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace NextHave.BL.Services.Rooms.Components
{
    [Service(ServiceLifetime.Scoped)]
    class RoomItemComponent(IServiceScopeFactory serviceScopeFactory) : IRoomComponent
    {
        IRoomInstance? _roomInstance;

        readonly ConcurrentDictionary<int, RoomItem> items = [];

        async Task IRoomComponent.Dispose()
        {
            if (_roomInstance == default)
                return;

            await _roomInstance.EventsService.UnsubscribeAsync<RoomTickEvent>(_roomInstance, OnRoomTick);

            await _roomInstance.EventsService.UnsubscribeAsync<LoadRoomItemsEvent>(_roomInstance, OnLoadRoomItems);

            await _roomInstance.EventsService.UnsubscribeAsync<SendItemsToNewUserEvent>(_roomInstance, OnSendItemsToNewUser);

            await _roomInstance.EventsService.UnsubscribeAsync<MoveObjectEvent>(_roomInstance, OnMoveObject);

            _roomInstance = default;
        }

        async Task IRoomComponent.Init(IRoomInstance roomInstance)
        {
            _roomInstance = roomInstance;

            await _roomInstance.EventsService.SubscribeAsync<RoomTickEvent>(_roomInstance, OnRoomTick);

            await _roomInstance.EventsService.SubscribeAsync<LoadRoomItemsEvent>(_roomInstance, OnLoadRoomItems);

            await _roomInstance.EventsService.SubscribeAsync<SendItemsToNewUserEvent>(_roomInstance, OnSendItemsToNewUser);

            await _roomInstance.EventsService.SubscribeAsync<MoveObjectEvent>(_roomInstance, OnMoveObject);
        }

        async Task OnRoomTick(RoomTickEvent @event)
        {
            await Task.CompletedTask;
        }

        async Task OnMoveObject(MoveObjectEvent @event)
        {
            if (_roomInstance?.Room == default || _roomInstance?.RoomModel == default)
                return;

            if (!items.TryGetValue(@event.ItemId, out var item))
                return;

            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            if (!item.Base!.AllowWalk)
                _roomInstance.RoomModel.SetWalkable(item.Point!.GetX, item.Point!.GetY, true);
            item.Point = new ThreeDPoint(@event.NewX, @event.NewY!, 0.0);
            if (!item.Base!.AllowWalk)
                _roomInstance.RoomModel.SetWalkable(item.Point!.GetX, item.Point!.GetY, false);

            var roomItem = await mongoDbContext.RoomItems.FirstOrDefaultAsync(i => i.EntityId == @event.ItemId && i.RoomId == _roomInstance.Room.Id)!;

            roomItem!.X = item.Point.GetX;
            roomItem.Y = item.Point.GetY;
            roomItem.Z = item.Point.GetZ;

            mongoDbContext.RoomItems.Update(roomItem);
            await mongoDbContext.SaveChangesAsync();
        }

        async Task OnLoadRoomItems(LoadRoomItemsEvent @event)
        {
            if (_roomInstance?.Room == default || _roomInstance?.RoomModel == default || !items.IsEmpty)
                return;

            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var itemsService = scope.ServiceProvider.GetRequiredService<IItemsService>();
            var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            var roomItems = await mongoDbContext.RoomItems.AsNoTracking().Where(i => i.RoomId == @event.RoomId).ToListAsync();

            Parallel.ForEach(roomItems, new()
            {
                CancellationToken = new(),
                MaxDegreeOfParallelism = Environment.ProcessorCount
            }, item =>
            {
                if (itemsService.TryGetItemDefinition(item.BaseId!.Value, out var itemDefinition))
                {
                    var result = item.Map(itemDefinition!, _roomInstance, 0);
                    items.TryAdd(item.EntityId!.Value, result);
                    if (itemDefinition!.Type == ItemTypes.Floor && !itemDefinition.AllowWalk)
                        foreach (var tile in result.Tiles)
                        {
                            _roomInstance.RoomModel.SetWalkable(tile.GetX, tile.GetY, false);
                        }
                }
            });
        }

        async Task OnSendItemsToNewUser(SendItemsToNewUserEvent @event)
        {
            if (_roomInstance?.Room == default || @event.Client == default)
                return;

            var floorItems = items.Values.Where(i => i.Base != default && i.Base.Type == ItemTypes.Floor).ToList();

            if (_roomInstance.Room.HideWired)
                floorItems = [.. floorItems.Where(i => !i.IsWired && !i.IsCondition)];

            await @event.Client.Send(new ObjectsMessageComposer(_roomInstance.Room.OwnerId!.Value, _roomInstance.Room.Owner!, floorItems));

            // TODO: wall items
        }
    }
}