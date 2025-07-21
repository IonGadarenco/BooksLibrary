using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using System.ComponentModel.DataAnnotations;

namespace BooksLibrary.API.Controllers.Dtos
{
    public class BookApiRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "ISBN is required")]
        public string ISBN { get; set; }
        [Required(ErrorMessage = "Total copies is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Total copies must be at least 1")]
        public int TotalCopies { get; set; }

        public IFormFile? CoverImageFile { get; set; }

        [Required(ErrorMessage = "Publisher details are required")]
        public PublisherDto Publisher { get; set; }
        [Required(ErrorMessage = "At least one author is required")]
        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
        [Required(ErrorMessage = "At least one category is required")]
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }
}
