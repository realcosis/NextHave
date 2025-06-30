using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.RoomToners)]
    [Index(nameof(RoomId), IsUnique = true)]
    [Index(nameof(ItemId), IsUnique = true)]
    public class RoomTonerEntity
    {
        [Required]
        public int? RoomId { get; set; }

        [Required]
        public int? ItemId { get; set; }

        [Required]
        public int? Hue { get; set; }

        [Required]
        public int? Saturation { get; set; }

        [Required]
        public int? Brightness { get; set; }

        public bool Enabled { get; set; }
    }
}