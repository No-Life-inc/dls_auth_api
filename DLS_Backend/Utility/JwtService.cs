using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace DLS_Backend.utility;

public class JwtService
{
    private readonly string _jwtSecret;

    /// <summary>
    /// Constructor for the JwtService
    /// </summary>
    /// <param name="jwtSecret"></param>
    public JwtService(string jwtSecret)
    {
        _jwtSecret = jwtSecret;
    }

    /// <summary>
    /// Generates a JWT token for the user with the given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GenerateToken(Guid id)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", id.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(Convert.FromBase64String(_jwtSecret)), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    //validate token
    public bool ValidateToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
        }
        catch (SecurityTokenException)
        {
            return false;
        }

        return true;
    }
    
}