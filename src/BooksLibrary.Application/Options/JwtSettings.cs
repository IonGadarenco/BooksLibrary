
namespace BooksLibrary.Application.Options
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public List<string> Audiences { get; set; }
    }
}
