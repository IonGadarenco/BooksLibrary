using Microsoft.AspNetCore.Mvc;

namespace BooksLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetAllCategories()
        {

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {

            

            return Ok();
        }

        [HttpPost]
        public IActionResult AddCategory()
        {
            
            return Ok();
        }

        [HttpPut]
        public IActionResult EditCategory()
        {
            

          

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            
            return Ok();
        }
    }
}
