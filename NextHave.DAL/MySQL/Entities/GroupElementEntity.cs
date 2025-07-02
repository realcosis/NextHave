using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.GroupElements)]
    public class GroupElementEntity
    {
        [Required]
        public int? Id { get; set; }

        [Required]
        public string? Type { get; set; }

        [Required]
        public string? Data1 { get; set; }

        [Required]
        public string? Data2 { get; set; }
    }
}