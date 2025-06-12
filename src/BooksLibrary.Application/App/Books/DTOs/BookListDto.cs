namespace BooksLibrary.Application.App.Books.DTOs
{
    public class BookListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
    }
}