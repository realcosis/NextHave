using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.ChatlogPrivates)]
    public class ChatlogPrivateEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int FromId { get; set; }

        [Required]
        public int ToId { get; set; }

        [ForeignKey(nameof(FromId))]
        public UserEntity? FromUser { get; set; }

        [ForeignKey(nameof(ToId))]
        public UserEntity? ToUser { get; set; }

        public ICollection<ChatlogPrivateDetailEntity> ChatlogPrivateDetails { get; set; } = [];
    }
}