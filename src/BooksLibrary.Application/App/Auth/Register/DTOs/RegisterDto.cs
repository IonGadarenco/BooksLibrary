using System.ComponentModel.DataAnnotations;
using BooksLibrary.Domain.Models;

namespace BooksLibrary.Application.App.Auth.Register.DTOs
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [EnumDataType(typeof(Roles))]
        public Roles Role { get; set; }
    }
}
