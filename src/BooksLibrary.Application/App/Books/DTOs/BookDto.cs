using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Categories.DOTs;

namespace BooksLibrary.Application.App.Books.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public int PublisherId { get; set; }
        public List<AuthorDto> Authors { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
}