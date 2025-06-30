using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Context;
using NextHave.BL.Events.Rooms;
using NextHave.BL.Events.Rooms.Session;
using NextHave.BL.Events.Rooms.Users;
using NextHave.BL.Events.Rooms.Users.Movements;
using NextHave.BL.Messages;
using NextHave.BL.Messages.Output.Rooms.Engine;
using NextHave.BL.Models;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Services.Rooms.Pathfinders;
using NextHave.BL.Utils;
using System.Collections.Concurrent;
using System.Text;

namespace NextHave.BL.Services.Rooms.Components
{
    [Service(ServiceLifetime.Scoped)]
    class RoomUserComponent(RoomUserFactory roomUserFactory) : IRoomComponent
    {
        IRoomInstance? _roomInstance;

        readonly ConcurrentDictionary<int, IRoomUserInstance> users = [];

        readonly ConcurrentDictionary<int, UserMovementContext> movementStates = [];

        int virtualId = 1;

        async Task IRoomComponent.Init(IRoomInstance roomInstance)
        {
            _roomInstance = roomInstance;

            await _roomInstance.EventsService.SubscribeAsync<RoomTickEvent>(_roomInstance, OnRoomTick);
            await _roomInstance.EventsService.SubscribeAsync<AddUserToRoomEvent>(_roomInstance, OnAddUserToRoomEvent);
            await _roomInstance.EventsService.SubscribeAsync<UserRoomExitEvent>(_roomInstance, OnUserExit);

            await _roomInstance.EventsService.SubscribeAsync<MoveAvatarEvent>(_roomInstance, OnMoveAvatarEvent);
            await _roomInstance.EventsService.SubscribeAsync<ProcessMovementEvent>(_roomInstance, OnProcessMovement);
            await _roomInstance.EventsService.SubscribeAsync<ApplyMovementEvent>(_roomInstance, OnApplyMovement);

            await _roomInstance.EventsService.SubscribeAsync<SendRoomPacketEvent>(_roomInstance, OnSendRoomPacketEvent);
        }

        async Task OnRoomTick(RoomTickEvent @event)
        {
            if (_roomInstance?.Room == default)
                return;

            var pendingMovements = movementStates.Where(m => m.Value.GoalPoint != default && !m.Value.IsProcessing).OrderBy(m => m.Value.RequestTime).ToList();

            foreach (var movement in pendingMovements)
                await _roomInstance.EventsService.DispatchAsync<ProcessMovementEvent>(new()
                {
                    RoomId = _roomInstance.Room.Id,
                    VirtualId = movement.Key,
                    RequestTime = movement.Value.RequestTime
                });

            var readyToMove = movementStates.Where(m => m.Value.HasPendingStep).Select(m => m.Key).ToList();

            foreach (var userId in readyToMove)
            {
                await _roomInstance.EventsService.DispatchAsync<ApplyMovementEvent>(new()
                {
                    RoomId = _roomInstance.Room.Id,
                    VirtualId = userId
                });
            }
        }

        async Task OnUserExit(UserRoomExitEvent @event)
        {
            if (_roomInstance == default || _roomInstance.RoomModel == default)
                return;

            var roomUserInstance = users.FirstOrDefault(u => u.Value.UserId == @event.UserId).Value;
            if (roomUserInstance == default)
                return;

            _roomInstance.RoomModel.RemoveUser(roomUserInstance.Position!, roomUserInstance);
            users.TryRemove(roomUserInstance.VirutalId, out var _);
            roomUserFactory.DestroyRoomUserInstance(roomUserInstance.UserId);

            await Send(new UserRemoveMessageComposer(roomUserInstance.VirutalId));
        }

        async Task OnApplyMovement(ApplyMovementEvent @event)
        {
            if (_roomInstance?.Room == default || _roomInstance?.RoomModel == default || !users.TryGetValue(@event.VirtualId, out var roomUserInstance) || !movementStates.TryGetValue(@event.VirtualId, out var state) || state.NextPoint == default || !state.HasPendingStep)
                return;

            var oldPosition = roomUserInstance.Position!.ToPoint();
            var newPosition = state.NextPoint;

            roomUserInstance.SetPosition(new ThreeDPoint(newPosition.GetX, newPosition.GetY, 0.0)); // TODO: calcolare Z
            _roomInstance.RoomModel.UpdateUser(oldPosition, newPosition, roomUserInstance);

            state.HasPendingStep = false;
            state.IsProcessing = false;

            await _roomInstance!.EventsService.DispatchAsync<MovementCompleteEvent>(new()
            {
                RoomId = _roomInstance.Room.Id,
                VirtualId = @event.VirtualId,
                Position = newPosition
            });
        }

        async Task OnProcessMovement(ProcessMovementEvent @event)
        {
            if (_roomInstance?.RoomModel == default || _roomInstance?.Room == default || !users.TryGetValue(@event.VirtualId, out var roomUserInstance))
                return;

            if (!movementStates.TryGetValue(@event.VirtualId, out var state) || state.GoalPoint == default)
                return;

            state.IsProcessing = true;

            var nextPoint = NextHavePathfinder.GetPath(_roomInstance.RoomModel, _roomInstance.Room.AllowDiagonal, roomUserInstance.Position!, state.GoalPoint);

            if (nextPoint == default)
            {
                roomUserInstance.RemoveStatus("mv");
                await SendUserUpdate(roomUserInstance);
                return;
            }

            state.NextPoint = nextPoint;
            state.HasPendingStep = true;

            PrepareMovement(roomUserInstance, nextPoint.ToThreeDPoint(0.0));
            await SendUserUpdate(roomUserInstance);
        }

        async Task OnMoveAvatarEvent(MoveAvatarEvent @event)
        {
            if (_roomInstance?.Room == default)
                return;

            var roomUserInstance = users.FirstOrDefault(u => u.Value.UserId == @event.UserId).Value;
            if (roomUserInstance == default)
                return;

            var state = movementStates.AddOrUpdate(roomUserInstance.VirutalId, new UserMovementContext
            {
                GoalPoint = new Point(@event.NewX!.Value, @event.NewY!.Value),
                RequestTime = DateTime.UtcNow,
                IsProcessing = false,
                HasPendingStep = false
            }, (key, existing) =>
            {
                existing.GoalPoint = new Point(@event.NewX!.Value, @event.NewY!.Value);
                existing.RequestTime = DateTime.UtcNow;
                existing.IsProcessing = false;
                return existing;
            });

            await Task.CompletedTask;
        }

        async Task OnAddUserToRoomEvent(AddUserToRoomEvent @event)
        {
            if (@event?.User?.Client == default || _roomInstance == default || _roomInstance?.Room == default || _roomInstance?.RoomModel == default)
                return;

            var roomUserInstance = roomUserFactory.GetRoomUserInstance(@event.User.Id, @event.User.Username!, virtualId++, @event.User, _roomInstance);

            roomUserInstance.SetPosition(new ThreeDPoint(_roomInstance.RoomModel.DoorX, _roomInstance.RoomModel.DoorY, _roomInstance.RoomModel.DoorZ));
            roomUserInstance.SetRotation(_roomInstance.RoomModel.DoorOrientation);

            _roomInstance.RoomModel.AddUser(roomUserInstance.Position!.ToPoint(), roomUserInstance);

            await roomUserInstance.Client!.Send(new UsersMessageComposer([.. users.Values]));

            users.TryAdd(roomUserInstance.VirutalId, roomUserInstance);

            await Send(new UsersMessageComposer([roomUserInstance]));
        }

        async Task OnSendRoomPacketEvent(SendRoomPacketEvent @event)
        {
            if (@event.Composer == default)
                return;

            await Send(@event.Composer);
        }

        async Task SendUserUpdate(IRoomUserInstance roomUserInstance)
            => await Send(new UserUpdateMessageComposer([roomUserInstance]));

        async Task Send(Composer message)
        {
            foreach (var client in users.Values.Select(u => u.Client).Where(c => c != default))
                await client!.Send(message);
        }

        static void PrepareMovement(IRoomUserInstance roomUserInstance, ThreeDPoint nextPoint)
        {
            roomUserInstance.RemoveStatus("mv");
            roomUserInstance.RemoveStatus("lay");
            roomUserInstance.RemoveStatus("sit");
            roomUserInstance.AddStatus("mv", $"{nextPoint.GetX},{nextPoint.GetY},{nextPoint.GetZ.GetString()}");
            var rotation = RotationCalculatorUtility.Calculate(roomUserInstance.Position!.GetX, roomUserInstance.Position!.GetY, nextPoint.GetX, nextPoint.GetY);
            roomUserInstance.SetRotation(rotation);
        }
    }
}