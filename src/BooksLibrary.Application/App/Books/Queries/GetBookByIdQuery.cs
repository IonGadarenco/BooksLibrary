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
            try
            {
                var book = await _bookRepository.GetByIdAsync(request.Id);

                if (book == null)
                {
                    _logger.LogError("Book with ID = {Id} does not exist", request.Id);
                    throw new EntityNotExistException("Book", request.Id);
                }

                _logger.LogInformation("Book with ID = {Id} exists", book.Id);
                return _mapper.Map<BookDto>(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the book with ID {Id}", request.Id);
                throw;
            }
        }
    }
}
