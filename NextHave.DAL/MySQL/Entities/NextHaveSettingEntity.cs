using NextHave.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.NextHaveSettings)]
    public class NextHaveSettingEntity
    {
        [Required]
        [MaxLength(128)]
        public string? Key { get; set; }

        [Required]
        [MaxLength(512)]
        public string? Value { get; set; }

        [Required]
        public SettingTypes? Type { get; set; }

        [MaxLength(1024)]
        public string? Description { get; set; }
    }
}