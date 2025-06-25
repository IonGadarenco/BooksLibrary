using AutoMapper;
using BooksLibrary.Application.App.Books.DTOs;
using BooksLibrary.Application.Common.Models;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Extensions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BooksLibrary.Application.App.Books.Queries
{
    public class GetPagedBooksQuery : IRequest<PaginatedResult<BookListDto>>
    {
        public PagedRequest PagedRequest { get; set; }
    }

    public class GetPagedBooksHandler : IRequestHandler<GetPagedBooksQuery, PaginatedResult<BookListDto>>
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

        public async Task<PaginatedResult<BookListDto>> Handle(GetPagedBooksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Start fetching paginated books. PageIndex={PageIndex}, PageSize={PageSize}, Sort={SortColumn} {SortDirection}, Filters={FilterCount}",
                    request.PagedRequest.PageIndex,
                    request.PagedRequest.PageSize,
                    request.PagedRequest.ColumnNameForSorting,
                    request.PagedRequest.SortDirection,
                    request.PagedRequest.RequestFilters.Filters.Count
                );

                var query = await _bookRepository.GetAllAsync();
                var result = await query.CreatePaginatedResultAsync<Book, BookListDto>(request.PagedRequest, _mapper);

                _logger.LogInformation("Fetched {ItemCount} books out of total {TotalCount}", result.Items.Count, result.TotalCount);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paginated books.");
                throw;
            }
        }
    }
}
