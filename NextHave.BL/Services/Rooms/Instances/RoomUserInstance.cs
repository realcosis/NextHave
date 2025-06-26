using NextHave.BL.Models;
using NextHave.BL.Clients;
using NextHave.BL.Messages;
using NextHave.BL.Models.Users;

namespace NextHave.BL.Services.Rooms.Instances
{
    class RoomUserInstance : IRoomUserInstance
    {
        IRoomUserInstance Instance => this;

        int IRoomUserInstance.UserId { get; set; }

        string? IRoomUserInstance.Username { get; set; }

        IRoomInstance? IRoomUserInstance.RoomInstance { get; set; }

        int IRoomUserInstance.VirutalId { get; set; }

        Client? IRoomUserInstance.Client { get; set; }

        User? IRoomUserInstance.User { get; set; }

        Point? Position { get; set; }

        public double Z { get; set; }

        public int BodyDirection { get; set; }

        void IRoomUserInstance.Serialize(ServerMessage message)
        {
            message.AddInt32(Instance.UserId);
            message.AddString(Instance.Username!);
            message.AddString(Instance.User!.Motto ?? string.Empty);
            message.AddString(Instance.User!.Look!);
            message.AddInt32(Instance.VirutalId);
            message.AddInt32(Position!.GetX);
            message.AddInt32(Position!.GetY);
            message.AddString(Z.ToString());
            message.AddInt32(BodyDirection);
            message.AddInt32(1);
            message.AddString(Instance.User.Gender!.ToLower());
            message.AddInt32(-1);
            message.AddInt32(-1);
            message.AddString(string.Empty);
            message.AddString(string.Empty);
            message.AddInt32(0);
            message.AddBoolean(false);
        }

        void IRoomUserInstance.SetData(int userId, string username, int virtualId, IRoomInstance roomInstance)
        {
            Instance.UserId = userId;
            Instance.Username = username;
            Instance.RoomInstance = roomInstance;
            Instance.VirutalId = virtualId;
        }

        void IRoomUserInstance.SetPosition(Point point, double z)
        {
            Position = point;
            Z = z;
        }
    }
}