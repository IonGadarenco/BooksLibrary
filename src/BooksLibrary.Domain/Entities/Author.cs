

namespace BooksLibrary.Domain.Models
{
    public class Author : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
