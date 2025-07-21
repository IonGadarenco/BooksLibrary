

namespace BooksLibrary.Application.App.Books.Commands.DTOs
{
    public class LoanDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; } // Data împrumutului
        public DateTime? ReturnedAt { get; set; } // Data returnării
    }
}
