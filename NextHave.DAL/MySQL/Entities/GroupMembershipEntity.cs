using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.GroupsMemberships)]
    [Index(nameof(GroupId))]
    [Index(nameof(UserId))]
    [Index(nameof(GroupId), nameof(UserId), IsUnique = true)]
    public class GroupMembershipEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int MemberRank { get; set; } = 3;

        [Required]
        public bool IsCurrent { get; set; }

        [Required]
        public bool IsPending { get; set; }

        public DateTime? JoinedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }

        [ForeignKey(nameof(GroupId))]
        public GroupEntity? Group { get; set; }
    }
}