using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.PermissionGroups)]
    public class PermissionGroupEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string? Name { get; set; }

        [MaxLength(256)]
        public string? Badge { get; set; }

        public ICollection<PermissionEntity> Permissions { get; set; } = [];
    }
}