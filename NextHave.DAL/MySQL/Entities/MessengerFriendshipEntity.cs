using NextHave.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.MessengerFriendships)]
    [Index(nameof(Sender), nameof(Receiver), IsUnique = true)]
    [Index(nameof(Receiver), nameof(Sender), IsUnique = true)]
    [Index(nameof(Sender))]
    [Index(nameof(Receiver))]
    public class MessengerFriendshipEntity
    {
        [Required]
        public int Sender { get; set; }

        [Required]
        public int Receiver { get; set; }

        [Required]
        public RelationshipStatus? Relationship { get; set; } = RelationshipStatus.None;

        [ForeignKey(nameof(Sender))]
        public UserEntity? SenderUser { get; set; }

        [ForeignKey(nameof(Receiver))]
        public UserEntity? ReceiverUser { get; set; }
    }
}