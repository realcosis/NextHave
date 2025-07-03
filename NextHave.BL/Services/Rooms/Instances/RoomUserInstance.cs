using NextHave.BL.Clients;
using NextHave.BL.Messages;
using NextHave.BL.Models;
using NextHave.BL.Models.Users;
using NextHave.BL.Services.Users.Instances;
using NextHave.BL.Utils;
using System.Collections.Concurrent;
using System.Text;

namespace NextHave.BL.Services.Rooms.Instances
{
    class RoomUserInstance : IRoomUserInstance
    {
        IRoomUserInstance Instance
            => this;

        int IRoomUserInstance.UserId { get; set; }

        string? IRoomUserInstance.Username { get; set; }

        IRoomInstance? IRoomUserInstance.RoomInstance { get; set; }

        Timer? IRoomUserInstance.Timer { get; set; }

        int IRoomUserInstance.VirutalId { get; set; }

        Client? IRoomUserInstance.Client { get; set; }

        IUserInstance? IRoomUserInstance.UserInstance { get; set; }

        ThreeDPoint? IRoomUserInstance.Position { get; set; }

        public int BodyDirection { get; set; }

        public int HeadDirection { get; set; }

        Point? IRoomUserInstance.GoalPoint { get; set; }

        ThreeDPoint? IRoomUserInstance.TempPoint { get; set; }

        bool IRoomUserInstance.IsWalking { get; set; }

        bool IRoomUserInstance.SetStep { get; set; }

        ConcurrentQueue<ServerMessage> IRoomUserInstance.Messages { get; } = [];

        readonly ConcurrentDictionary<string, string> Status = [];

        void IRoomUserInstance.AddStatus(string key, string value)
            => Status.AddOrUpdate(key, value, (_, oldValue) => value);

        void IRoomUserInstance.RemoveStatus(string key)
            => Status.TryRemove(key, out  _);

        bool IRoomUserInstance.HasStatus(string key)
            => Status.ContainsKey(key);

        string IRoomUserInstance.GetStatus(string key)
        {
            if (Instance.HasStatus(key))
                return Status[key];
            else
                return string.Empty;
        }

        void IRoomUserInstance.Serialize(ServerMessage message)
        {
            message.AddInt32(Instance.UserId);
            message.AddString(Instance.Username!);
            message.AddString(Instance.UserInstance!.User!.Motto ?? string.Empty);
            message.AddString(Instance.UserInstance.User.Look!);
            message.AddInt32(Instance.VirutalId);
            message.AddInt32(Instance.Position!.GetX);
            message.AddInt32(Instance.Position!.GetY);
            message.AddString(Instance.Position.GetZ.GetString());
            message.AddInt32(BodyDirection);
            message.AddInt32(1);
            message.AddString(Instance.UserInstance.User.Gender!.ToLower());
            message.AddInt32(-1);
            message.AddInt32(-1);
            message.AddString(string.Empty);
            message.AddString(string.Empty);
            message.AddInt32(0);
            message.AddBoolean(false);
        }

        void IRoomUserInstance.SerializeStatus(ServerMessage message)
        {
            var stringBuilder = new StringBuilder();
            message.AddInt32(Instance.VirutalId);
            message.AddInt32(Instance.Position!.GetX);
            message.AddInt32(Instance.Position!.GetY);
            message.AddString(Instance.Position!.GetZ.GetString());
            message.AddInt32(HeadDirection);
            message.AddInt32(BodyDirection);
            stringBuilder.Append('/');
            foreach (var state in Status)
            {
                stringBuilder.Append(state.Key);
                if (!string.IsNullOrWhiteSpace(state.Value))
                {
                    stringBuilder.Append(' ');
                    stringBuilder.Append(state.Value);
                }
                stringBuilder.Append('/');
            }
            if (Instance.HasStatus("sign"))
                Instance.RemoveStatus("sign");
            stringBuilder.Append('/');
            message.AddString(stringBuilder.ToString());
        }

        void IRoomUserInstance.SetData(int userId, string username, int virtualId, IRoomInstance roomInstance)
        {
            Instance.UserId = userId;
            Instance.Username = username;
            Instance.RoomInstance = roomInstance;
            Instance.VirutalId = virtualId;
        }

        void IRoomUserInstance.SetRotation(int direction)
        {
            BodyDirection = direction;
            HeadDirection = direction;
        }

        void IRoomUserInstance.SetPosition(ThreeDPoint point)
            => Instance.Position = point;
    }
}