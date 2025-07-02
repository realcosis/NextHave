using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.ChatlogRooms)]
    public class ChatlogRoomEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int? UserId { get; set; }

        [Required]
        public int? RoomId { get; set; }

        [Required]
        public string? Message { get; set; }

        [Required]
        public DateTime? Datetime { get; set; }
    }
}