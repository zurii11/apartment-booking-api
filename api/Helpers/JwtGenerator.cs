using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ApartmentBooking.Helpers;

public class JwtGenerator : IJwtGenerator
{
    private readonly IConfiguration _config;

    public JwtGenerator(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(int user_id, string username)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user_id.ToString()),
            new Claim(ClaimTypes.Name, username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
                );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
