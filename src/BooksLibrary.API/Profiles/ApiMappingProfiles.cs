using AutoMapper;
using BooksLibrary.API.Controllers.Dtos;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.App.Books.Commands.DTOs;

namespace BooksLibrary.API.Profiles
{
    public class ApiMappingProfiles : Profile
    {
        public ApiMappingProfiles()
        {
            CreateMap<BookApiRequestDto, CreateBookCommand>()
                .ForMember(d => d.CoverImageUrl, opt => opt.Ignore())
                .ForMember(d => d.CoverImageFile, opt => opt.MapFrom(src => src.CoverImageFile));

            CreateMap<UpdateBookCommandDto, UpdateBookCommand>();
        }
    }
    
    
}
