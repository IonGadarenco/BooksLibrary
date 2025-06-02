


namespace BooksLibrary.Domain.Models
{
    public class Publisher : Entity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
