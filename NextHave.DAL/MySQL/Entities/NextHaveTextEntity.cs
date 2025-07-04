using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.NextHaveTexts)]
    public class NextHaveTextEntity
    {
        [Key]
        [MaxLength(128)]
        public string? Key { get; set; }

        [Required]
        [MaxLength(512)]
        public string? Value { get; set; }

        [Required]
        [MaxLength(1024)]
        public string? Description { get; set; }
    }
}