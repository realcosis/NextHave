using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.NavigatorUserCategories)]
    public class NavigatorUserCategoryEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public bool Enabled { get; set; }

        public int MinRank { get; set; } = 1;
    }
}