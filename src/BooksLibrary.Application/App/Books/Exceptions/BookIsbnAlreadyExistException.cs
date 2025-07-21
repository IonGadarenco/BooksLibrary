

namespace BooksLibrary.Application.App.Books.Exceptions
{
    public class BookIsbnAlreadyExistException : Exception
    {
        public BookIsbnAlreadyExistException(string isbn):base($"Book ISBN [{isbn}] already exist!") { }
    }
}
