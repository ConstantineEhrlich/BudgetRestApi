using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public class JwtGenerator
{
    private readonly IConfiguration _configuration;

    public JwtGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtKey(User user, uint daysUntilExpiration)
    {
        SymmetricSecurityKey jwtKey = new(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"]));
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityTokenDescriptor descriptor = new()
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.Name, user.Login),
            }),
            Expires = DateTime.UtcNow.AddDays(daysUntilExpiration),
            SigningCredentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken token = tokenHandler.CreateToken(descriptor);
        return tokenHandler.WriteToken(token);
    }
}