using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextHave.DAL.MySQL.Entities
{
    [Table(Tables.Groups)]
    public class GroupEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public string? Image { get; set; }

        [Required]
        public int CustomPrimaryColor { get; set; }

        [Required]
        public int CustomSecondaryColor { get; set; }

        [Required]
        public int Base { get; set; }

        [Required]
        public int BaseColor { get; set; }

        [Required]
        public string HtmlPrimaryColor { get; set; } = "ffffff";

        [Required]
        public string? HtmlSecondaryColor { get; set; } = "ffffff";

        [Required]
        public string? CreationDate { get; set; }

        [Required]
        public string Petitions { get; set; } = "";

        [Required]
        public int Type { get; set; } = 0;

        [Required]
        public int RightsType { get; set; } = 0;

        [Required]
        public int WhoCanRead { get; set; } = 0;

        [Required]
        public int WhoCanPost { get; set; } = 1;

        [Required]
        public int WhoCanThread { get; set; } = 1;

        [Required]
        public int WhoCanMod { get; set; } = 2;

        public ICollection<GroupMembershipEntity> Members { get; set; } = [];
    }
}