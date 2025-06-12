

namespace BooksLibrary.Domain.Models
{
    public class Author : Entity
    {
        public string FullName { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
        
    }
}
