using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.UserFavorites)]
    [Index(nameof(UserId), nameof(RoomId), IsUnique = true)]
    [Index(nameof(UserId))]
    [Index(nameof(RoomId))]
    public class UserFavoriteEntity
    {
        [Required]
        public int? UserId { get; set; }

        [Required]
        public int? RoomId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
    }
}