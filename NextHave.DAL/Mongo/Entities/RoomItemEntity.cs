using MongoDB.Bson;
using Newtonsoft.Json;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NextHave.DAL.Mongo.Entities.Items;

namespace NextHave.DAL.Mongo.Entities
{
    [Collection(Tables.RoomItems)]
    public class RoomItemEntity
    {
        [Key]
        [JsonProperty("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ObjectId? Id { get; set; }

        [Required]
        public int? EntityId { get; set; }

        [Required]
        public int? RoomId { get; set; }

        [Required]
        public int? BaseId { get; set; }

        [Required]
        public int? X {  get; set; }
        
        [Required]
        public int? Y { get; set; }

        [Required]
        public double? Z { get; set; }

        [Required]
        public int? Rotation { get; set; }

        public string? WallPosition { get; set; }

        public string? ExtraData { get; set; }

        public ItemLimitedMongoEntity? Limited { get; set; }

        public ItemValidRareMongoEntity? ValidRare { get; set; }
    }
}