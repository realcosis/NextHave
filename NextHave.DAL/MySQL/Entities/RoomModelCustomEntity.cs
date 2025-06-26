using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.RoomModelCustoms)]
    public class RoomModelCustomEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RoomId { get; set; }

        [Required]
        public int? DoorX { get; set; }

        [Required]
        public int? DoorY { get; set; }

        [Required]
        public double? Height { get; set; }

        [Required]
        public int? DoorDir { get; set; }

        [Required]
        public string? ModelData { get; set; }
    }
}