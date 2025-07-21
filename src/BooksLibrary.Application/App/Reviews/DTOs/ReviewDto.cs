

namespace BooksLibrary.Application.App.Reviews.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string UserName { get; set; } // Vom afișa numele, nu ID-ul
        public DateTime CreatedAt { get; set; } // Data la care a fost postat
    }
}
