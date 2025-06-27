using Dolphin.Core.Events;
using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Events.Rooms;
using NextHave.BL.Events.Users;
using NextHave.BL.Messages;
using NextHave.BL.Messages.Output;
using NextHave.BL.Models;
using NextHave.BL.Models.Rooms.Movements;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Utils;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Channels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NextHave.BL.Services.Rooms.Components
{
    [Service(ServiceLifetime.Scoped)]
    class RoomUserComponent(RoomUserFactory roomUserFactory, IEventsService eventsService) : IRoomComponent
    {
        IRoomInstance? _roomInstance;

        readonly ConcurrentDictionary<int, IRoomUserInstance> users = [];

        readonly ConcurrentDictionary<int, UserMovementData> userMovements = [];

        int virtualId = 1;

        readonly SemaphoreSlim sendLock = new(1, 1);

        readonly ConcurrentQueue<ServerMessage> messages = [];

        async Task IRoomComponent.Init(IRoomInstance roomInstance)
        {
            _roomInstance = roomInstance;

            await roomInstance.EventsService.SubscribeAsync<AddUserToRoomEvent>(roomInstance, OnAddUserToRoomEvent);

            await roomInstance.EventsService.SubscribeAsync<MoveAvatarEvent>(roomInstance, OnMoveAvatarEvent);

            await roomInstance.EventsService.SubscribeAsync<RoomTickEvent>(roomInstance, OnRoomTick);

            await roomInstance.EventsService.SubscribeAsync<RequestPathEvent>(roomInstance, OnRequestPathEvent);

            await roomInstance.EventsService.SubscribeAsync<PathCalculatedEvent>(roomInstance, OnPathCalculatedEvent);

            await eventsService.SubscribeAsync<UserDisconnectedEvent>(roomInstance, OnUserDisconnect);
        }

        void EnqueueUpdate(ServerMessage message)
            => messages.Enqueue(message);
        
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

        async Task OnRoomTick(RoomTickEvent @event)
        {
            if (_roomInstance?.Room == default)
                return;

            if (messages.TryDequeue(out var packet))
                await Send(packet);

            //var updates = new List<IRoomUserInstance>();

            //foreach (var userMovement in userMovements)
            //{
            //    var userId = userMovement.Key;
            //    var movementData = userMovement.Value;

            //    if (!movementData.IsMoving || movementData.Points.Count == 0)
            //        continue;

            //    var roomUser = users.Values.FirstOrDefault(u => u.UserId == userId);
            //    if (roomUser == null)
            //        continue;

            //    if (movementData.NextPoint == default)
            //    {
            //        if (movementData.CurrentPathIndex < movementData.Points.Count)
            //        {
            //            movementData.NextPoint = movementData.Points[movementData.CurrentPathIndex];
            //            movementData.CurrentPathIndex++;

            //            PrepareNextMove(roomUser, movementData.NextPoint);
            //            updates.Add(roomUser);
            //        }
            //    }
            //    else
            //    {
            //        var z = 0.0;
            //        var oldPosition = new Point(roomUser.Position!.GetX, roomUser.Position!.GetY);
            //        var newPosition = new ThreeDPoint(movementData.NextPoint.GetX, movementData.NextPoint.GetY, z);

            //        _roomInstance.RoomModel.UpdateUser(oldPosition, movementData.NextPoint, roomUser);

            //        roomUser.SetPosition(newPosition);

            //        if (movementData.CurrentPathIndex < movementData.Points.Count)
            //        {
            //            movementData.NextPoint = movementData.Points[movementData.CurrentPathIndex];
            //            movementData.CurrentPathIndex++;

            //            PrepareNextMove(roomUser, movementData.NextPoint);
            //            updates.Add(roomUser);
            //        }
            //        else
            //        {
            //            movementData.IsMoving = false;
            //            roomUser.RemoveStatus("mv");
            //            updates.Add(roomUser);
            //            userMovements.TryRemove(userId, out _);
            //        }
            //    }
            //}

            //if (updates.Count > 0)
            //{
            //    await using var updateMessage = ServerMessageFactory.GetServerMessage(OutputCode.UserUpdateMessageComposer);
            //    updateMessage.AddInt32(updates.Count);
            //    foreach (var user in updates)
            //    {
            //        user.SerializeStatus(updateMessage);
            //    }
            //    await Send(updateMessage);
            //}
        }

        async Task OnMoveAvatarEvent(MoveAvatarEvent @event)
        {
            if (_roomInstance?.Room == default || _roomInstance?.Pathfinder == default || _roomInstance?.RoomModel == default)
                return;

            var roomUserInstance = users.FirstOrDefault(u => u.Value.UserId == @event.UserId).Value;
            if (roomUserInstance == default)
                return;

            roomUserInstance.GoalPoint = new Point(@event.NewX!.Value, @event.NewY!.Value);

            await _roomInstance.EventsService.DispatchAsync<RequestPathEvent>(new()
            {
                RoomId = _roomInstance.Room.Id,
                RoomUserInstance = roomUserInstance,
                NewX = @event.NewX,
                NewY = @event.NewY
            });
        }

        async Task OnRequestPathEvent(RequestPathEvent @event)
        {
            if (@event.RoomUserInstance == default || _roomInstance?.Room == default || _roomInstance?.Pathfinder == default || _roomInstance?.RoomModel == default)
                return;

            var points = _roomInstance.Pathfinder.FindPath(@event.RoomUserInstance.Position!.GetX, @event.RoomUserInstance.Position!.GetY, @event.NewX!.Value, @event.NewY!.Value, _roomInstance.Room.AllowDiagonal);

            await _roomInstance.EventsService.DispatchAsync<PathCalculatedEvent>(new()
            {
                Point = points.FirstOrDefault(),
                RoomId = @event.RoomId,
                RoomUserInstance = @event.RoomUserInstance
            });
        }

        async Task OnPathCalculatedEvent(PathCalculatedEvent @event)
        {
            if (_roomInstance?.Room == default || _roomInstance?.RoomModel == default || @event.RoomUserInstance == default || @event.RoomUserInstance.GoalPoint == default)
                return;

            @event.RoomUserInstance.RemoveStatus("mv");
            if (@event.Point == default)
            {
                await SendUserUpdate(@event.RoomUserInstance);
                return;
            }

            var z = 0.0;
            var prev = @event.RoomUserInstance.Position!;

            var mvStatus = $"{@event.Point.GetX},{@event.Point.GetY},{z.GetString()}";
            @event.RoomUserInstance.AddStatus("mv", mvStatus);

            var rotation = RotationCalculatorUtility.Calculate(prev.GetX, prev.GetY, @event.Point.GetX, @event.Point.GetY);

            _roomInstance.RoomModel.UpdateUser(prev, @event.Point, @event.RoomUserInstance);
            @event.RoomUserInstance.SetPosition(@event.Point.ToThreeDPoint(z));
            @event.RoomUserInstance.SetRotation(rotation);

            await SendUserUpdate(@event.RoomUserInstance);

            await _roomInstance.EventsService.DispatchAsync<RequestPathEvent>(new()
            {
                RoomId = _roomInstance.Room.Id,
                RoomUserInstance = @event.RoomUserInstance,
                NewX = @event.RoomUserInstance.GoalPoint.GetX,
                NewY = @event.RoomUserInstance.GoalPoint.GetY
            });
        }

        //async Task OnMoveAvatarEvent(MoveAvatarEvent @event)
        //{
        //    if (_roomInstance?.Room == default || _roomInstance?.Pathfinder == default || _roomInstance?.RoomModel == default)
        //        return;

        //    var roomUserInstance = users.FirstOrDefault(u => u.Value.UserId == @event.UserId).Value;
        //    if (roomUserInstance == default)
        //        return;

        //    var z = 0.0;
        //    if (roomUserInstance.GoalPoint != default && !roomUserInstance.GoalPoint.Equals(new Point(@event.NewX!.Value, @event.NewY!.Value)))
        //    {
        //        roomUserInstance.SetPosition(roomUserInstance.TempPoint!.ToThreeDPoint(z));
        //        userMovements.TryRemove(@event.UserId!.Value, out _);
        //        roomUserInstance.RemoveStatus("mv");
        //    }

        //    if (userMovements.TryGetValue(@event.UserId!.Value, out var currentMovement) && currentMovement.IsMoving)
        //    {
        //        var currentPosition = default(Point?);
        //        if (currentMovement.CurrentPathIndex > 0 && currentMovement.CurrentPathIndex <= currentMovement.Points.Count)
        //            currentPosition = currentMovement.Points[currentMovement.CurrentPathIndex - 1];
        //        else
        //            currentPosition = new Point(roomUserInstance.Position!.GetX, roomUserInstance.Position!.GetY);

        //        var newPos = new ThreeDPoint(currentPosition.GetX, currentPosition.GetY, z);

        //        _roomInstance.RoomModel.UpdateUser(roomUserInstance.Position!, newPos, roomUserInstance);

        //        roomUserInstance.SetPosition(newPos);
        //        userMovements.TryRemove(@event.UserId!.Value, out _);
        //        roomUserInstance.RemoveStatus("mv");
        //        await using var stopMessage = ServerMessageFactory.GetServerMessage(OutputCode.UserUpdateMessageComposer);
        //        stopMessage.AddInt32(1);
        //        roomUserInstance.SerializeStatus(stopMessage);
        //        await Send(stopMessage);
        //    }

        //    var points = _roomInstance.Pathfinder.FindPath(roomUserInstance.Position!.GetX, roomUserInstance.Position!.GetY, @event.NewX!.Value, @event.NewY!.Value, _roomInstance.Room.AllowDiagonal);

        //    if (points.Count == 0)
        //        return;

        //    var movementData = new UserMovementData
        //    {
        //        Points = points,
        //        CurrentPathIndex = 0,
        //        IsMoving = true,
        //        NextPoint = null
        //    };
        //    roomUserInstance.GoalPoint = new Point(@event.NewX.Value, @event.NewY.Value);

        //    userMovements[@event.UserId!.Value] = movementData;
        //}

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

        async Task Send(ServerMessage message)
        {
            var buffer = message.Bytes();

            foreach (var client in users.Values.Select(u => u.Client).Where(c => c != null))
                await client!.Send(buffer);
        }

        #region private

        private async Task SendUserUpdate(IRoomUserInstance roomUserInstance)
        {
            var userUpdateMessageComposer = ServerMessageFactory.GetServerMessage(OutputCode.UserUpdateMessageComposer);
            userUpdateMessageComposer.AddInt32(1);
            roomUserInstance.SerializeStatus(userUpdateMessageComposer);
            //await Send(userUpdateMessageComposer);
            EnqueueUpdate(userUpdateMessageComposer);
        }

        #endregion
    }
}