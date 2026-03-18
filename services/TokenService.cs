using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AutentikaatioAutorisaatio.Services
{

    public class TokenService
    {
        public string GenerateToken(string username, bool isAdmin)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_that_is_at_least_32_bytes_long"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };

            var roleClaim = isAdmin
                ? new Claim(ClaimTypes.Role, "Admin")
                : new Claim(ClaimTypes.Role, "User");
            claims.Add(roleClaim);

            var tokeOptions = new JwtSecurityToken(
                issuer: "MyTestAuthServer",
                audience: "MyTestApiUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signinCredentials
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }
    }
}