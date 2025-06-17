

namespace BooksLibrary.Application.App.Books.DTOs
{
    public class UpdateBookCommandDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int PublisherId { get; set; }
    }
}
