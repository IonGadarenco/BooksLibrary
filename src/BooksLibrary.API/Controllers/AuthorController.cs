using Microsoft.AspNetCore.Mvc;

namespace BooksLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        public List<Author> Authors = new List<Author>()
        {
            new Author
            {
                Id = 1,
                Name = "Cristian Barbosu",
            },
            new Author
            {
                Id = 2,
                Name = "Ioan Bunacui"
            }
        };

        [HttpGet]
        public IActionResult GetAuthors()
        {
            return Ok(Authors);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthorById(int id)
        {
            var author = Authors.FirstOrDefault(a => a.Id == id);

            if (author == null)
            {
                return BadRequest();
            }

            return Ok(author);
        }

        [HttpPost]
        public IActionResult AddAuthor(Author author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingAuthor = Authors.FirstOrDefault(a => a.Id == author.Id);
            if (existingAuthor != null)
            {
                return BadRequest("Author exist!");
            }

            Authors.Add(author);

            return Ok(Authors);
        }

        [HttpPut]
        public IActionResult UpdateAuthor(Author author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingAuthor = Authors.FirstOrDefault(a => a.Id == author.Id);

            if (existingAuthor == null)
            {
                return NotFound("Author not existing");
            }

            Authors.Remove(existingAuthor);
            Authors.Add(author);

            return Ok(Authors);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthors(int id)
        {
            var author = Authors.FirstOrDefault(a => a.Id == id);

            if (author == null)
            {
                return NotFound("Author not found");
            }

            Authors.Remove(author);
            return Ok(Authors);
        }
    }
}
