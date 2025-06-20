using BooksLibrary.Application.App.Auth.Abstractions;

namespace BooksLibrary.Application.Options
{
    public class JwtSettings
    {
        public string SigningKey { get; set; }
        public string Issuer { get; set; }
        public string[] Audiences { get; set; }
    }
}
