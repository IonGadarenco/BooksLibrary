

namespace BooksLibrary.Domain.Models
{
    public class Reservation : Entity
    {
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
