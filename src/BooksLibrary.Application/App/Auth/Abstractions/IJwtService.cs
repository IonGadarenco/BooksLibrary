
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace BooksLibrary.Application.App.Auth.Abstractions
{
    public interface IJwtService
    {
        SecurityToken CreateSecurityToken(ClaimsIdentity identity);
        string WriteToken(SecurityToken token);
    }
}
