using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DigiBuy.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using DigiBuy.Application.Token;

namespace DigiBuy.Application.Services.Implementations;

public class JwtTokenService
{
    private readonly JwtSettings jwtSettings;
        
    public JwtTokenService(JwtSettings jwtSettings)
    {
        this.jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
    }   

    public string GenerateToken(User user)
    {
        if (user == null) 
        {
            throw new ArgumentNullException(nameof(user));
        }
            
        var claims = GetClaims(user);
        var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtToken = new JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpiration),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        );

        return tokenHandler.WriteToken(jwtToken);
    }
        
    private Claim[] GetClaims(User user)
    {
        return new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        }.ToArray();
    }
}