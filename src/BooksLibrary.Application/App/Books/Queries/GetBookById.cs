using AutoMapper;
using BooksLibrary.Application.App.Books.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;

namespace BooksLibrary.Application.App.Books.Queries
{
    public class GetBookById : IRequest<BookDto>
    {
        public int Id { get; set; }
    }
    public class GetBookByIdHandler : IRequestHandler<GetBookById, BookDto>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        public GetBookByIdHandler(IRepository<Book> bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }
        public async Task<BookDto> Handle(GetBookById request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id);
            return _mapper.Map<BookDto>(book);
        }
    }
}
