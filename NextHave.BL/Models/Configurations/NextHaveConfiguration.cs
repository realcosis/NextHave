using Dolphin.Core.Configurations.Models;

namespace NextHave.BL.Models.Configurations
{
    public class NextHaveConfiguration : Configuration
    {
        public JwtConfiguration? Jwt { get; set; }
    }
}