using AutoMapper;
using BooksLibrary.Application.App.Books.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BooksLibrary.Application.App.Books.Queries
{
    public class GetBookByIdQuery : IRequest<BookDto>
    {
        public int Id { get; set; }
    }
    public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, BookDto>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBookByIdHandler> _logger;
        public GetBookByIdHandler(IRepository<Book> bookRepository, IMapper mapper, ILogger<GetBookByIdHandler> logger)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            
            var book = await _bookRepository.GetByIdAsync(request.Id);

            if (book == null)
            {
                throw new EntityNotExistException("Book", request.Id);
            }

            _logger.LogInformation("Book with ID = {Id} exists", book.Id);
            return _mapper.Map<BookDto>(book);
            
        }
    }
}
