using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Armut.Messaging.Infrastructure.Services
{
    public static class TokenService
    {
        public static string Create(ClaimsIdentity claimsIdentity, string key, DateTime? expiresOn = null)
        {
            var handler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(key);

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claimsIdentity,
                Expires = expiresOn.HasValue ? expiresOn.Value : DateTime.UtcNow.AddHours(8),
                SigningCredentials = signingCredentials,
            };

            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);

        }
    }
}
