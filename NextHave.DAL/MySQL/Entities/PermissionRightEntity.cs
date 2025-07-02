using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.PermissionRights)]
    public class PermissionRightEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string? Name { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }

        public ICollection<PermissionEntity> Permissions { get; set; } = [];
    }
}