using System.Text;
using Microsoft.JSInterop;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using NextHave.BL.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using NextHave.BL.Services.Users;

namespace NextHave.Nitro.Authentications
{
    class NextHaveAuthenticationStateProvider(IJSRuntime jsRuntime, IConfiguration configuration, IUsersService usersService) : AuthenticationStateProvider
    {
        readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());

        ClaimsPrincipal? currentUser;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            currentUser = default;

            var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "sessiontoken");

            var userIsAuthenticated = !string.IsNullOrWhiteSpace(token);

            if (!userIsAuthenticated)
                return new AuthenticationState(anonymous);

            if (currentUser == default)
            {
                var tokenValue = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var userIdVal = tokenValue.Claims.FirstOrDefault(c => c.Type == NextHaveAuthenticationConstants.Id)?.Value;
                if (!string.IsNullOrWhiteSpace(userIdVal) && int.TryParse(userIdVal, out var userId))
                {
                    try
                    {
                        var user = await usersService.GetFromToken(userId);
                        if (user != default)
                        {
                            var claims = new List<Claim>()
                            {
                                new(NextHaveAuthenticationConstants.Username, user.Username!),
                                new(NextHaveAuthenticationConstants.Id, user.Id.ToString())
                            };
                            var identity = new ClaimsIdentity(claims, "NextHave");
                            var principal = new ClaimsPrincipal(identity);
                            currentUser = principal;
                        }
                    }
                    catch
                    {
                        return new AuthenticationState(anonymous);
                    }
                }
                else
                    return new AuthenticationState(anonymous);
            }

            return new AuthenticationState(currentUser!);
        }

        public void NotifyUserAuthentication(ClaimsPrincipal user)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            currentUser = user;
        }

        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(NextHaveAuthenticationConstants.Username, user.Username!),
                new Claim(NextHaveAuthenticationConstants.Id, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"]!,
                audience: configuration["Jwt:Audience"]!,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}