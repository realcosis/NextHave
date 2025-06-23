using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.NavigatorPublicCategories)]
    public class NavigatorPublicCategoryEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int OrderNumber { get; set; } = 0;

        [Required]
        public string? Name { get; set; }
        
        public bool Visible { get; set; }
    }
}