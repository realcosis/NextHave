using NextHave.BL.Models;
using NextHave.BL.Clients;
using NextHave.BL.Messages;
using System.Collections.Concurrent;
using NextHave.BL.Services.Users.Instances;

namespace NextHave.BL.Services.Rooms.Instances
{
    public interface IRoomUserInstance
    {
        public Client? Client { get; set; }

        public Timer? Timer { get; set; }

        public IUserInstance? UserInstance { get; set; }

        public Point? GoalPoint { get; set; }

        public ThreeDPoint? TempPoint { get; set; }

        public ConcurrentQueue<ServerMessage> Messages { get; }

        public ThreeDPoint? Position { get; set; }

        public int UserId { get; set; }

        public string? Username { get; set; }

        public IRoomInstance? RoomInstance { get; set; }

        public int VirutalId { get; set; }
        public bool IsWalking { get; set; }

        public bool SetStep { get; set; }

        public void SetData(int userId, string username, int virtualId, IRoomInstance roomInstance);

        public void Serialize(ServerMessage message);

        public void SerializeStatus(ServerMessage message);

        public void SetPosition(ThreeDPoint point);

        public void SetRotation(int direction);

        void AddStatus(string key, string value);

        void RemoveStatus(string key);

        string GetStatus(string key);

        bool HasStatus(string key);
    }
}