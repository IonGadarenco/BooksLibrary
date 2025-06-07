using Microsoft.AspNetCore.Mvc;

namespace BooksLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        public List<Book> Books = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Title = "Amintiri din copilarie",
                    Description = "Povestiri amuzante din copilariea lui Ion Creanga",
                    Author = "Ion Creanga"
                },
                new Book
                {
                    Id = 2,
                    Title = "Biserica Verticala",
                    Description = "Teologiea eclesiologica",
                    Author = "Josh McDonald"
                }
            };

        [HttpGet]
        public IActionResult GetAllBooks() 
        {
            var books = Books;

            return Ok(books);
        }

        [HttpGet("{id}")]
        public IActionResult GetBookById(int id)
        {
            var book = Books.FirstOrDefault(b => b.Id == id);

            if( book is null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        public IActionResult AddBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(book);
        }

        [HttpPut]
        public IActionResult EditBook(Book book)
        {
            if (book == null)
            {
                return BadRequest("Book data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBook = Books.FirstOrDefault(b => b.Id == book.Id);
            if (existingBook == null)
            {
                return NotFound("Book not found.");
            }

            Books.Remove(existingBook);
            Books.Add(book);

            return Ok(Books);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var book = Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            Books.Remove(book);
            return Ok(Books);
        }
    }
}
