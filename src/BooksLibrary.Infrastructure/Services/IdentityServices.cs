using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BooksLibrary.Application.App.Auth.Abstractions;
using BooksLibrary.Application.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BooksLibrary.Infrastructure.Services
{
    public class IdentityServices : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly byte[] _key;

        public IdentityServices(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
            ArgumentNullException.ThrowIfNull(_jwtSettings);
            ArgumentNullException.ThrowIfNull(_jwtSettings.SigningKey);
            ArgumentNullException.ThrowIfNull(_jwtSettings.Issuer);
            ArgumentNullException.ThrowIfNull(_jwtSettings.Audiences);
            ArgumentNullException.ThrowIfNull(_jwtSettings.Audiences[0]);
            _key = Encoding.ASCII.GetBytes(_jwtSettings.SigningKey);
        }

        private static JwtSecurityTokenHandler TokenHandler => new();

        public SecurityToken CreateSecurityToken(ClaimsIdentity identity)
        {
            var tokenDescriptor = GetTokenDescriptor(identity);

            return TokenHandler.CreateToken(tokenDescriptor);
        }

        public string WriteToken(SecurityToken token)
        {
            return TokenHandler.WriteToken(token);
        }

        private SecurityTokenDescriptor GetTokenDescriptor(ClaimsIdentity identity)
        {
            return new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audiences[0],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };
        }
    }
}
