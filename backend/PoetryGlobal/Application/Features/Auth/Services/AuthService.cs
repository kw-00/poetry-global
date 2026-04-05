using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using PoetryGlobal.Application.Exceptions;

namespace PoetryGlobal.Features.Auth
{
    public class AuthService(IConfiguration configuration) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;

        public string GenerateJwtToken()
        {
            var jwtConfig = _configuration.GetSection("Jwt");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };

            var issuer = jwtConfig["Issuer"] ?? throw new AppSettingsKeyNotFound("Jwt.Issuer");
            var lifetimeMinutes = int.Parse(jwtConfig["LifetimeMinutes"] 
                ?? throw new AppSettingsKeyNotFound("Jwt.LifetimeMinutes"));

            var token = new JwtSecurityToken(
                issuer: issuer,
                expires: DateTime.UtcNow.AddMinutes(lifetimeMinutes),
                signingCredentials: _GetSigningCredentials(),
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private SigningCredentials _GetSigningCredentials()
        {
            string secret = Environment.GetEnvironmentVariable("JWT_SECRET") 
                ?? throw new EnvironmentVariableNotSetException("JWT_SECRET");
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
    }
}