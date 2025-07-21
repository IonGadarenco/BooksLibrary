


namespace BooksLibrary.Domain.Models
{
    public class Publisher : Entity
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();

    }
}
