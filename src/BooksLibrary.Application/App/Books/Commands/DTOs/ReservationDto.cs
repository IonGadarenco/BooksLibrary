

namespace BooksLibrary.Application.App.Books.Commands.DTOs
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; } // Data rezervării
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}
