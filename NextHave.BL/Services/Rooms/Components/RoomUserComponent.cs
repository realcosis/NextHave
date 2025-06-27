using Dolphin.Core.Events;
using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Context;
using NextHave.BL.Contexts;
using NextHave.BL.Events.Rooms;
using NextHave.BL.Events.Rooms.Movements;
using NextHave.BL.Events.Users;
using NextHave.BL.Messages;
using NextHave.BL.Messages.Output;
using NextHave.BL.Models;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Utils;
using System.Collections.Concurrent;
using System.Text;

namespace NextHave.BL.Services.Rooms.Components
{
    [Service(ServiceLifetime.Scoped)]
    class RoomUserComponent(RoomUserFactory roomUserFactory, IEventsService eventsService) : IRoomComponent
    {
        IRoomInstance? _roomInstance;

        readonly ConcurrentDictionary<int, IRoomUserInstance> users = [];

        readonly ConcurrentDictionary<Point, TileReservationContext> tileReservations = [];

        readonly ConcurrentDictionary<int, UserMovementContext> movementStates = [];

        int virtualId = 1;

        async Task IRoomComponent.Init(IRoomInstance roomInstance)
        {
            _roomInstance = roomInstance;

            await roomInstance.EventsService.SubscribeAsync<RoomTickEvent>(roomInstance, OnRoomTick);
            await eventsService.SubscribeAsync<UserDisconnectedEvent>(roomInstance, OnUserDisconnect);
            await roomInstance.EventsService.SubscribeAsync<AddUserToRoomEvent>(roomInstance, OnAddUserToRoomEvent);

            await roomInstance.EventsService.SubscribeAsync<MoveAvatarEvent>(roomInstance, OnMoveAvatarEvent);
            await roomInstance.EventsService.SubscribeAsync<ProcessMovementEvent>(roomInstance, OnProcessMovement);
            await roomInstance.EventsService.SubscribeAsync<ApplyMovementEvent>(roomInstance, OnApplyMovement);
            await roomInstance.EventsService.SubscribeAsync<MovementCompleteEvent>(roomInstance, OnMovementComplete);
        }

        async Task OnRoomTick(RoomTickEvent @event)
        {
            if (_roomInstance?.Room == default)
                return;

            var expiredTime = DateTime.UtcNow.AddSeconds(-2);
            var expiredReservations = tileReservations.Where(r => !r.Value.IsConfirmed && r.Value.ReservationTime < expiredTime).Select(r => r.Key).ToList();

            foreach (var tile in expiredReservations)
                tileReservations.TryRemove(tile, out _);

            var pendingMovements = movementStates.Where(m => m.Value.GoalPoint != null && !m.Value.IsProcessing).OrderBy(m => m.Value.RequestTime).ToList();

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

        async Task OnUserDisconnect(UserDisconnectedEvent @event)
        {
            if (_roomInstance == default || _roomInstance.RoomModel == default)
                return;

            var roomUserInstance = users.FirstOrDefault(u => u.Value.UserId == @event.UserId).Value;
            if (roomUserInstance == default)
                return;

            _roomInstance.RoomModel.RemoveUser(roomUserInstance.Position!, roomUserInstance);
            users.TryRemove(roomUserInstance.VirutalId, out var _);
            roomUserFactory.DestroyRoomUserInstance(roomUserInstance.UserId);

            await using var userRemoveMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.UserRemoveMessageComposer);
            userRemoveMessageComposer.AddString(roomUserInstance.VirutalId.ToString());
            await Send(userRemoveMessageComposer);
        }

        async Task OnApplyMovement(ApplyMovementEvent @event)
        {
            if (_roomInstance?.Room == default || _roomInstance?.RoomModel == default || _roomInstance?.Pathfinder == default || !users.TryGetValue(@event.VirtualId, out var user) || !movementStates.TryGetValue(@event.VirtualId, out var state) || state.NextPoint == default || !state.HasPendingStep)
                return;

            var oldPosition = user.Position!.ToPoint();
            var newPosition = state.NextPoint;

            if (tileReservations.TryGetValue(newPosition, out var reservation) && reservation.VirtualId == @event.VirtualId)
                reservation.IsConfirmed = true;

            if (tileReservations.TryGetValue(oldPosition, out var oldReservation) && oldReservation.VirtualId == @event.VirtualId)
                tileReservations.TryRemove(oldPosition, out _);

            user.SetPosition(new ThreeDPoint(newPosition.GetX, newPosition.GetY, 0.0)); // TODO: calcolare Z

            _roomInstance.RoomModel.UpdateUser(oldPosition, newPosition, user);

            state.HasPendingStep = false;
            state.IsProcessing = false;

            await _roomInstance!.EventsService.DispatchAsync<MovementCompleteEvent>(new()
            {
                RoomId = _roomInstance.Room.Id,
                VirtualId = @event.VirtualId,
                Position = newPosition
            });
        }

        async Task OnMovementComplete(MovementCompleteEvent @event)
        {
            if (!users.TryGetValue(@event.VirtualId, out var user) || !movementStates.TryGetValue(@event.VirtualId, out var state))
                return;

            if (state.GoalPoint != null && @event.Position!.Equals(state.GoalPoint))
            {
                state.GoalPoint = null;
                user.RemoveStatus("mv");
                await SendUserUpdate(user);
            }
        }

        async Task OnProcessMovement(ProcessMovementEvent @event)
        {
            if (_roomInstance?.Pathfinder == null || _roomInstance?.RoomModel == null || _roomInstance?.Room == null || !users.TryGetValue(@event.VirtualId, out var roomUserInstance))
                return;

            if (!movementStates.TryGetValue(@event.VirtualId, out var state) || state.GoalPoint == default)
                return;

            state.IsProcessing = true;

            var nextPoint = _roomInstance.Pathfinder.FindPath(roomUserInstance.Position!.GetX, roomUserInstance.Position!.GetY, state.GoalPoint.GetX, state.GoalPoint.GetY).FirstOrDefault();

            if (nextPoint == default || nextPoint.Equals(roomUserInstance.Position))
            {
                state.GoalPoint = null;
                state.IsProcessing = false;
                roomUserInstance.RemoveStatus("mv");
                await SendUserUpdate(roomUserInstance);
                return;
            }

            if (CanMoveToTile(nextPoint, @event.VirtualId))
            {
                ReserveTile(nextPoint, @event.VirtualId);
                state.NextPoint = nextPoint;
                state.HasPendingStep = true;

                PrepareMovement(roomUserInstance, nextPoint.ToThreeDPoint(0.0));
                await SendUserUpdate(roomUserInstance);
            }
            else
            {
                var alternative = FindAlternativePath(roomUserInstance, nextPoint);
                if (alternative != default)
                {
                    ReserveTile(alternative, @event.VirtualId);
                    state.NextPoint = alternative;
                    state.HasPendingStep = true;

                    PrepareMovement(roomUserInstance, alternative.ToThreeDPoint(0.0));
                    await SendUserUpdate(roomUserInstance);
                }
                else
                    state.IsProcessing = false;
            }
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
            if (@event?.User?.Client == default || _roomInstance == default || _roomInstance.RoomModel == default)
                return;

            @event.User.CurrentRoomInstance = _roomInstance;

            var roomUser = roomUserFactory.GetRoomUserInstance(@event.User.Id, @event.User.Username!, virtualId++, @event.User, _roomInstance);

            roomUser.SetPosition(new ThreeDPoint(_roomInstance.RoomModel.DoorX, _roomInstance.RoomModel.DoorY, _roomInstance.RoomModel.DoorZ));
            roomUser.SetRotation(_roomInstance.RoomModel.DoorOrientation);

            _roomInstance.RoomModel.AddUser(roomUser.Position!.ToPoint(), roomUser);

            await using var usersMessageComposerToUser = ServerMessageFactory.GetServerMessage(OutputCode.UsersMessageComposer);
            usersMessageComposerToUser.AddInt32(users.Count);
            foreach (var user in users.Values)
                user.Serialize(usersMessageComposerToUser);
            await roomUser.Client!.Send(usersMessageComposerToUser.Bytes());

            users.TryAdd(roomUser.VirutalId, roomUser);

            await using var usersMessageComposerToRoom = ServerMessageFactory.GetServerMessage(OutputCode.UsersMessageComposer);
            usersMessageComposerToRoom.AddInt32(1);
            roomUser.Serialize(usersMessageComposerToRoom);
            await Send(usersMessageComposerToRoom);
        }

        async Task SendUserUpdate(IRoomUserInstance user)
        {
            await using var updateMessage = ServerMessageFactory.GetServerMessage(OutputCode.UserUpdateMessageComposer);
            updateMessage.AddInt32(1);
            user.SerializeStatus(updateMessage);
            await Send(updateMessage);
        }

        async Task Send(ServerMessage message)
        {
            var buffer = message.Bytes();

            foreach (var client in users.Values.Select(u => u.Client).Where(c => c != default))
                await client!.Send(buffer);
        }

        Point? FindAlternativePath(IRoomUserInstance roomUserInstance, Point point)
        {
            if (_roomInstance?.Pathfinder == default || _roomInstance?.RoomModel == default || _roomInstance?.Room == default || !movementStates.TryGetValue(roomUserInstance.UserId, out var state) || state.GoalPoint == default)
                return default;

            var pathFromAlternative = _roomInstance.Pathfinder.FindPath(roomUserInstance.Position!.GetX, roomUserInstance.Position.GetY, state.GoalPoint.GetX, state.GoalPoint.GetY).FirstOrDefault();
            if (pathFromAlternative != default)
                return pathFromAlternative;

            return default;
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

        bool CanMoveToTile(Point tile, int virtualId)
        {
            if (!tileReservations.TryGetValue(tile, out var reservation))
                return true;

            return reservation.VirtualId == virtualId || (!reservation.IsConfirmed && reservation.ReservationTime < DateTime.UtcNow.AddSeconds(-1));
        }

        void ReserveTile(Point tile, int virtualId)
            => tileReservations.AddOrUpdate(tile, new TileReservationContext
            {
                VirtualId = virtualId,
                ReservationTime = DateTime.UtcNow,
                IsConfirmed = false
            }, (key, existing) =>
            {
                existing.VirtualId = virtualId;
                existing.ReservationTime = DateTime.UtcNow;
                existing.IsConfirmed = false;
                return existing;
            });
    }
}