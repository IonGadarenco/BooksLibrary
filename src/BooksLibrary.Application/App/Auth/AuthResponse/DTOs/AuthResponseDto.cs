

using BooksLibrary.Domain.Models;

namespace BooksLibrary.Application.App.Auth.AuthResponse.DTOs
{
    public class AuthResponseDto
    {
        public string Email { get; set; }
        public Roles Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}
