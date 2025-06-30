using NextHave.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.ItemDefinitions)]
    public class ItemDefinitionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int? SpriteId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? ItemName { get; set; }

        [Required]
        public ItemTypes? Type { get; set; }

        [Required]
        public int? Width { get; set; }

        [Required]
        public double? Height { get; set; }

        [Required]
        public int? Length { get; set; }

        public bool AllowStack { get; set; } = true;

        public bool AllowSit { get; set; } = false;

        public bool AllowWalk { get; set; } = false;

        public bool AllowRecycle { get; set; } = true;

        public bool AllowTrade { get; set; } = true;

        public bool AllowMarketPlaceSell { get; set; } = true;

        public bool AllowGift { get; set; } = true;

        public bool AllowInventoryStack { get; set; } = true;

        [Required]
        public string? InteractionType { get; set; } = "default";

        [Required]
        public int? InteractionCount { get; set; } = 0;
    }
}