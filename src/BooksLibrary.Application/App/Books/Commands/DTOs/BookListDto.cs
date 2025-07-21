using BooksLibrary.Domain.Models;

namespace BooksLibrary.Application.App.Books.Commands.DTOs
{
    public class BookListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Author> Authors { get; set; }
        public int TotalCopies { get; set; }
        public string CoverImageUrl { get; set; }
    }
}