using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.RoomModels)]
    public class RoomModelEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string? Id { get; set; }

        [Required]
        public int? DoorX { get; set; }

        [Required]
        public int? DoorY { get; set; }

        [Required]
        public double? DoorZ { get; set; }

        [Required]
        public int? DoorDir { get; set; }

        [Required]
        public string? HeightMap { get; set; }
    }
}