using MongoDB.Bson;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.Mongo.Entities
{
    [Table(Tables.CatalogPages)]
    public class CatalogPageEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("id")]
        public ObjectId Id { get; set; }

        [Required]
        public int? EntityId { get; set; }

        public int? ParentId { get; set; }

        [Required]
        public string? Name { get; set; }

        public bool Enabled { get; set; }

        [Required]
        public int? RequiredRank { get; set; }

        [Required]
        public int? OrderNumber { get; set; }

        [Required]
        public string? Layout { get; set; }

        public List<CatalogItemEntity> Items { get; set; } = [];
    }
}