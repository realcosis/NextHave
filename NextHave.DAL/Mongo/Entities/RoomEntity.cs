using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using Newtonsoft.Json;
using NextHave.DAL.Enums;
using NextHave.DAL.Mongo.Entities.Rooms;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.Mongo.Entities
{
    [Collection(Tables.Rooms)]
    public class RoomEntity
    {
        [Key]
        [JsonProperty("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ObjectId? Id { get; set; }

        [Required]
        public int? EntityId { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public RoomAuthorEntity? Author { get; set; }

        [Required]
        public RoomModelEntity? Model { get; set; }

        [Required]
        public RoomCategoryEntity? Category { get; set; }

        public RoomGroupEntity? Group { get; set; }

        public int? MaxUsers { get; set; } = 25;

        [Required]
        public RoomAccessStatus? AccessStatus { get; set; }

        public int? Score { get; set; } = 0;

        public List<string> Tags { get; set; } = [];

        public string? Password { get; set; }

        public Dictionary<string, double> Bans { get; set; } = [];

        public List<uint> Rights { get; set; } = [];

        public bool AllowPets { get; set; }

        public bool AllowPetsEat { get; set; }

        public bool AllowWalkthrough { get; set; }

        public bool AllowHidewall { get; set; }

        public bool AllowRightsOverride { get; set; }

        public bool HideWired { get; set; }

        public int WallThickness { get; set; }

        public int FloorThickness { get; set; }

        public bool? AllowDiagonal { get; set; }

        public int MuteSettings { get; set; }

        public int BanSettings { get; set; }

        public int KickSettings { get; set; }

        public int TradeSettings { get; set; }

        public int WallHeight { get; set; }

        public int RollerSpeed { get; set; }

        public string? Wallpaper { get; set; }

        public string? Floorpaper { get; set; }

        public string? Landscape { get; set; }
    }
}