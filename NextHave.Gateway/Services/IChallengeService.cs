namespace NextHave.Gateway.Services
{
    public interface IChallengeService
    {
        string GenerateChallenge(string ip);

        bool ValidateChallenge(string ip, string response);

        bool IsWhitelisted(string ip);
    }
}