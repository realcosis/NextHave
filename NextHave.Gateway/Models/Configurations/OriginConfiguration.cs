using Dolphin.Core.Configurations.Models;

namespace NextHave.Gateway.Models.Configurations
{
    public class OriginConfiguration : Configuration
    {
        public string[] Hosts { get; set; } = [];
    }
}