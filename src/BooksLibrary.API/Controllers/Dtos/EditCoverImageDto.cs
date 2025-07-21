using Microsoft.AspNetCore.Mvc;

namespace BooksLibrary.API.Controllers.Dtos
{
    public class EditCoverImageDto
    {
        [FromForm(Name = "coverImageFile")]
        public IFormFile CoverImageFile { get; set; }

        [FromForm(Name = "coverImageUrlToDelete")]
        public string CoverImageUrlToDelete { get; set; }
    }
}
