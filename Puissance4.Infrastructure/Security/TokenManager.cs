using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Puissance4.Infrastructure.Security
{
    public class TokenManager(
        JwtSecurityTokenHandler _handler,
        TokenManager.Config  _config
    )
    {

        public class Config
        {
            public string Signature { get; set; } = null!;
        }

        public SecurityKey SecurityKey { 
            get { return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Signature)); } 
        }

        public string CreateToken(int id, string username)
        {
            JwtSecurityToken token = new(
                null,
                null,
                [new Claim(ClaimTypes.NameIdentifier, id.ToString()), new Claim(ClaimTypes.Name, username)],
                null,
                null,
                new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256)
            );

            return _handler.WriteToken(token);
        }

        public ClaimsPrincipal? Validate(string token)
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SecurityKey,
            };
            try
            {
                return _handler.ValidateToken(token, validationParameters, out SecurityToken t);
            }
            catch (Exception) 
            { 
                return null;
            }
        }
    }
}
