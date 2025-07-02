using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.Permissions)]
    [Index(nameof(GroupId))]
    [Index(nameof(RightId))]
    [Index(nameof(GroupId), nameof(RightId), IsUnique = true)]
    public class PermissionEntity
    {
        [Required]
        public int? GroupId { get; set; }

        [Required]
        public int? RightId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public PermissionGroupEntity? Group { get; set; }

        [ForeignKey(nameof(RightId))]
        public PermissionRightEntity? Right { get; set; }
    }
}