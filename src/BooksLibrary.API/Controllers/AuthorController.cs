using Microsoft.AspNetCore.Mvc;

namespace BooksLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        

        [HttpGet]
        public IActionResult GetAuthors()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthorById(int id)
        {

            
            

            return Ok();
        }

        [HttpPost]
        public IActionResult AddAuthor()
        {
            


            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateAuthor()
        {
            

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthors(int id)
        {

            

            return Ok();
        }
    }
}
