using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Application.App.Books.Exceptions
{
    public class BookIsbnAlreadyExistException : Exception
    {
        public BookIsbnAlreadyExistException(string isbn):base($"Book ISBN [{isbn}] already exist!") { }
    }
}
