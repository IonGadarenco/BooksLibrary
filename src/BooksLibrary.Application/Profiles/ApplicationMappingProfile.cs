using AutoMapper;
using BooksLibrary.Application.App.Auth.AuthResponse.DTOs;
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.Domain.Models;

namespace BooksLibrary.Application.Mappings
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));
            CreateMap<Loan, LoanDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));
            CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));
            CreateMap<Book, PublicBookDetailsDto>()
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews))
            .ForMember(dest => dest.AvailableCopies, opt => opt.MapFrom(src =>
                src.TotalCopies -
                (src.Loans != null ? src.Loans.Count(l => l.ReturnedAt == null) : 0) - 
                (src.Reservations != null ? src.Reservations.Count(r => r.IsActive) : 0) 
            ));
            CreateMap<Book, AdminBookDetailsDto>()
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews))
            .ForMember(dest => dest.Loans, opt => opt.MapFrom(src => src.Loans.Where(l => l.ReturnedAt == null))) 
            .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Reservations.Where(r => r.IsActive))) 
            .ForMember(dest => dest.AvailableCopies, opt => opt.MapFrom(src =>
                src.TotalCopies -
                (src.Loans != null ? src.Loans.Count(l => l.ReturnedAt == null) : 0) -
                (src.Reservations != null ? src.Reservations.Count(r => r.IsActive) : 0)
            ));
            CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));
            CreateMap<UpdateBookCommandDto, UpdateBookCommand>().ReverseMap();
            CreateMap<Book, BookListDto>().ReverseMap();
            CreateMap<Book, UpdateBookCommandDto>().ReverseMap();
            CreateMap<Author, AuthorDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Publisher, PublisherDto>().ReverseMap();
            CreateMap<User, AuthResponseDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ReverseMap();
        }
    }
}
