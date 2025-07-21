

using BooksLibrary.Domain.Entities;

namespace BooksLibrary.Domain.Models
{
    public enum Roles
    {
        admin,
        vip,
        user
    }
    public class User : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Auth0Id { get; set; }
        public Roles Role { get; set; } = Roles.user;
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<UserLike> Likes { get; set; } = new List<UserLike>();

    }
}
