
namespace BooksLibrary.Domain.Models
{
    public class Book : Entity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
        public Reservation Reservation { get; set; }
        public ICollection<Author> Authors { get; set; } = new List<Author>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }   
}
