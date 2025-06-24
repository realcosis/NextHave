using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.UserTickets)]
    [Index(nameof(Ticket), IsUnique = true)]
    [Index(nameof(UserId), IsUnique = true)]
    public class UserTicketEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UserId { get; set; }

        [Required]
        [MaxLength(512)]
        public string? Ticket { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
    }
}
