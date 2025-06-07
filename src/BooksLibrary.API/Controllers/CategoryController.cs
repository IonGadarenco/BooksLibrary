using Microsoft.AspNetCore.Mvc;

namespace BooksLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        public List<Category> Categories = new List<Category>
            {
                new Category
                {
                    Id = 1,
                    Name = "teologie"
                },
                new Category
                {
                    Id = 2,
                    Name = "familia",
                }
            };

        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var categories = Categories;

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var category = Categories.FirstOrDefault(c => c.Id == id);

            if (category is null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(category);
        }

        [HttpPut]
        public IActionResult EditCategory(Category category)
        {
            if (category == null)
            {
                return BadRequest("Category data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCategory = Categories.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }

            Categories.Remove(existingCategory);
            Categories.Add(category);

            return Ok(Categories);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            Categories.Remove(category);
            return Ok(Categories);
        }
    }
}
