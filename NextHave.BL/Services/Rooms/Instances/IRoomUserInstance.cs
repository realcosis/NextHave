using NextHave.BL.Models;
using NextHave.BL.Clients;
using NextHave.BL.Messages;
using NextHave.BL.Models.Users;

namespace NextHave.BL.Services.Rooms.Instances
{
    public interface IRoomUserInstance
    {
        public Client? Client { get; set; }

        public User? User { get; set; }

        public Point? GoalPoint { get; set; }

        public Point? TempPoint { get; set; }

        public ThreeDPoint? Position { get; set; }

        public int UserId { get; set; }

        public string? Username { get; set; }

        public IRoomInstance? RoomInstance { get; set; }

        public int VirutalId { get; set; }

        public void SetData(int userId, string username, int virtualId, IRoomInstance roomInstance);

        public void Serialize(ServerMessage message);

        public void SerializeStatus(ServerMessage message);

        public void SetPosition(ThreeDPoint point);

        public void SetRotation(int direction);

        void AddStatus(string key, string value);

        void RemoveStatus(string key);

        bool HasStatus(string key);
    }
}