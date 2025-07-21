

namespace BooksLibrary.Application.App.Books.Commands.DTOs
{
    public class AdminBookDetailsDto : BookDetailsBaseDto
    {
        public List<LoanDto> Loans { get; set; }
        public List<ReservationDto> Reservations { get; set; }
    }
}
