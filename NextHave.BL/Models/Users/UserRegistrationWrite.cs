using Dolphin.Core.Validations;

namespace NextHave.BL.Models.Users
{
    public class UserRegistrationWrite
    {
        [Required]
        [MaxLength(64)]
        public string? Username { get; set; }

        [Required]
        [MaxLength(12)]
        [MinLength(6)]
        public string? Password { get; set; }

        [Required]
        [MaxLength(12)]
        [MinLength(6)]
        public string? ConfirmPassword { get; set; }

        [Required]
        [Email]
        [MaxLength(512)]
        public string? Mail { get; set; }
    }
}