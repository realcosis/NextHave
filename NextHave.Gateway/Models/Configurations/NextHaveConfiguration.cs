using Newtonsoft.Json.Linq;
using Dolphin.Core.Configurations.Models;

namespace NextHave.Gateway.Models.Configurations
{
    public class NextHaveConfiguration : Configuration
    {
        public OriginConfiguration? Origin { get; set; }

        public JObject? ReverseProxy { get; set; }
    }
}