using NextHave.DAL.Enums;

namespace NextHave.BL.Models.Items
{
    public class ItemDefinition
    {
        public int Id { get; set; }

        public int? SpriteId { get; set; }

        public string? Name { get; set; }

        public string? ItemName { get; set; }

        public ItemTypes? Type { get; set; }

        public int? Width { get; set; }

        public double? Height { get; set; }

        public int? Length { get; set; }

        public bool AllowStack { get; set; }

        public bool AllowSit { get; set; }

        public bool AllowWalk { get; set; } 

        public bool AllowRecycle { get; set; }

        public bool AllowTrade { get; set; }

        public bool AllowMarketPlaceSell { get; set; }

        public bool AllowGift { get; set; } = true;

        public bool AllowInventoryStack { get; set; }

        public string? InteractionType { get; set; }

        public int? InteractionCount { get; set; }

        public bool IsRareItem { get; set; }
    }
}