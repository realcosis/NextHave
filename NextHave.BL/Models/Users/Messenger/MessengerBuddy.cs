using Microsoft.EntityFrameworkCore.Query;
using NextHave.BL.Clients;
using NextHave.BL.Messages;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.DAL.Enums;
using System.Threading;

namespace NextHave.BL.Models.Users.Messenger
{
    public class MessengerBuddy
    {
        public int UserId { get; set; }

        public string? Username { get; set; }

        public string? Look { get; set; }

        public string? Motto { get; set; }

        public RelationshipStatus? Relationship { get; set; }

        public IRoomInstance? CurrentRoom { get; set; }

        public Client? Client { get; set; }

        public bool Room 
            => CurrentRoom != default;

        public bool Online
            => Sessions.ConnectedClients.Values.Any(c => c.UserInstance?.User != default && c.UserInstance.User.Id == UserId);

        public void Serialize(ServerMessage message, User user)
        {
            message.AddInt32(UserId);
            message.AddString(Username!);
            message.AddInt32(1);
            message.AddBoolean(Online);
            message.AddBoolean(Room);
            message.AddString(Online ? Look! : string.Empty);
            message.AddInt32(0);
            message.AddString(Motto!);
            message.AddString(string.Empty);
            message.AddString(string.Empty);
            message.AddBoolean(true);
            message.AddBoolean(false);
            message.AddBoolean(false);
            message.AddInt16((short)Relationship!);
        }
    }
}