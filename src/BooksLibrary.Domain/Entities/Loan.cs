

namespace BooksLibrary.Domain.Models
{
    public class Loan : Entity
    {
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
