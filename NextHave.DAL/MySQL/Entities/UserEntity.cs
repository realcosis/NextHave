using NextHave.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.Users)]
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string? Username { get; set; }

        public string? Motto { get; set; }

        public int Rank { get; set; } = 1;

        [Required]
        public string? Look { get; set; }

        public GenderTypes? Gender { get; set; }

        public bool Online { get; set; }

        public DateTime? LastOnline { get; set; }

        public int? HomeRoom { get; set; }

        public ICollection<UserTicketEntity> Tickets { get; set; } = [];
    }
}