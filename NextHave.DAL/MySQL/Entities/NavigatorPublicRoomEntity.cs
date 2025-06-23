using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.NavigatorPublicRooms)]
    public class NavigatorPublicRoomEntity
    {
        [Required]
        public int OrderNumber { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RoomId { get; set; }

        [Required]
        public string? Thumbnail { get; set; }
    }
}