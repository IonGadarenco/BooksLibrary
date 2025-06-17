using AutoMapper;
using BooksLibrary.Application.App.Books.DTOs;
using BooksLibrary.Application.Common.Models;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BooksLibrary.Application.App.Books.Queries
{
    public class GetPagedBooksQuery : IRequest<List<BookListDto>>
    {
        public PagedRequest PagedRequest { get; set; } = default!;
    }

    public class GetPagedBooksHandler : IRequestHandler<GetPagedBooksQuery, List<BookListDto>>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPagedBooksHandler> _logger;

        public GetPagedBooksHandler(
            IRepository<Book> bookRepository,
            IMapper mapper,
            ILogger<GetPagedBooksHandler> logger)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<BookListDto>> Handle(GetPagedBooksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching paged books: PageIndex={PageIndex}, PageSize={PageSize}",
                    request.PagedRequest.PageIndex, request.PagedRequest.PageSize);

                var books = await _bookRepository.GetAllAsyncPaged(
                    request.PagedRequest.PageIndex,
                    request.PagedRequest.PageSize);

                var bookDtos = _mapper.Map<List<BookListDto>>(books);

                _logger.LogInformation("Successfully fetched {Count} books", bookDtos.Count);

                return bookDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching paged books.");
                throw;
            }
        }
    }
}
