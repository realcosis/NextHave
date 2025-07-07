using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.ChatlogPrivateDetails)]
    public class ChatlogPrivateDetailEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ChatlogId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string? Message { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [ForeignKey(nameof(ChatlogId))]
        public ChatlogPrivateEntity? Chatlog { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
    }
}