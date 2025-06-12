using AutoMapper;
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.DTOs;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.Domain.Models;

namespace BooksLibrary.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<Author, AuthorDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Publisher, PublisherDto>().ReverseMap();
        }
    }
}
