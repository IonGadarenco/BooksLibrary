

namespace BooksLibrary.Domain.Models
{
    public class Review : Entity
    {
        public string Comment { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
