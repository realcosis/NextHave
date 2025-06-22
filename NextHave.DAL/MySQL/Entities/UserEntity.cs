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

        [Required]
        [MaxLength(128)]
        public string? Password { get; set; }

        [Required]
        [MaxLength(512)]
        public string? Mail { get; set; }

        public string? Motto { get; set; }

        [Required]
        public int? Rank { get; set; }

        [Required]
        public string? Look { get; set; }

        [Required]
        public GenderTypes? Gender { get; set; }

        public bool Online { get; set; }

        public DateTime? LastOnline { get; set; }

        public int? HomeRoom { get; set; }

        [Required]
        [MaxLength(64)]
        public string? Volume { get; set; }

        [Required]
        public DateTime? AccountCreated { get; set; }

        [Required]
        [MaxLength(512)]
        public string? RegistrationIp { get; set; }

        [Required]
        [MaxLength(512)]
        public string? CurrentIp { get; set; }

        public ICollection<UserTicketEntity> Tickets { get; set; } = [];
    }
}