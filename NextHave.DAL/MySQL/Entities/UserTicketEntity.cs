using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.UserTickets)]
    [Index(nameof(UserId), nameof(Ticket), IsUnique = true)]
    public class UserTicketEntity
    {
        [Required]
        public int? UserId { get; set; }

        [Required]
        [MaxLength(512)]
        public string? Ticket { get; set; }

        public DateTime? UsedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
    }
}
