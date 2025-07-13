using Microsoft.Extensions.Caching.Memory;

namespace NextHave.Gateway.Services
{
    class ChallengeService(IMemoryCache cache) : IChallengeService
    {
        private readonly HashSet<string> whitelist = ["127.0.0.1", "::1"];

        string IChallengeService.GenerateChallenge(string ip)
        {
            var challenge = Guid.NewGuid().ToString();
            cache.Set($"challenge_{ip}", challenge, TimeSpan.FromMinutes(5));
            return challenge;
        }

        bool IChallengeService.ValidateChallenge(string ip, string response)
        {
            if (cache.TryGetValue($"challenge_{ip}", out string? expectedChallenge))
            {
                cache.Remove($"challenge_{ip}");
                return expectedChallenge == response;
            }
            return false;
        }

        bool IChallengeService.IsWhitelisted(string ip)
            => whitelist.Contains(ip);
    }
}