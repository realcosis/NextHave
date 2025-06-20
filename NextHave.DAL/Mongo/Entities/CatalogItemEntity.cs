using NextHave.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace NextHave.DAL.Mongo.Entities
{
    public class CatalogItemEntity
    {
        [Required]
        public int? ItemId { get; set; }

        [Required]
        public string? PublicName { get; set; }

        [Required]
        public int? CostCredits { get; set; }

        [Required]
        public int? CostPoints { get; set; }

        [Required]
        public PointTypes? PointType { get; set; }

        [Required]
        public int? Amount { get; set; }
    }
}