using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace BooksLibrary.Application.App.Books.Commands.DTOs
{
    public class UpdateBookCommandDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PublisherDto Publisher { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
        public string CoverImageUrl { get; set; }
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }
}
