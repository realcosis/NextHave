using NextHave.BL.Validations;

namespace NextHave.BL.Models.Users
{
    public class UserLoginWrite
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}