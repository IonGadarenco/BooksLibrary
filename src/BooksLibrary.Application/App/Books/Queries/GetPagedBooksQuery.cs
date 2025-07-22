
using System.Linq.Expressions;
using AutoMapper;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.Common.Models;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

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
                _logger.LogInformation("Start fetching paginated books. PageIndex={PageIndex}, PageSize={PageSize}, Sort={SortColumn} {SortDirection}",
                    request.PagedRequest.PageIndex,
                    request.PagedRequest.PageSize,
                    request.PagedRequest.ColumnNameForSorting,
                    request.PagedRequest.SortDirection
                );

                var searchValue = request.PagedRequest.SearchValue;

                var query = await _bookRepository.GetAllAsync();

                query = query.Where(FilterBy(request.PagedRequest.SearchBy, searchValue));

                var totalCount = query.Count();
                var totalPages = request.PagedRequest != null ? (int)Math.Ceiling((double)totalCount / request.PagedRequest.PageSize) : 1;

                var result = await query.OrderBy($"{request.PagedRequest.ColumnNameForSorting} {request.PagedRequest.SortDirection}")
                        .Skip((request.PagedRequest.PageIndex - 1) * request.PagedRequest.PageSize)
                        .Take(request.PagedRequest.PageSize)
                        .ProjectTo<BookListDto>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                _logger.LogInformation("Fetched {ItemCount} books out of total {TotalCount}", result.Count, totalCount);

                 return new PaginatedResult<BookListDto>
                {
                    Items = result,
                    PageIndex = request.PagedRequest.PageIndex,
                    PageSize = request.PagedRequest.PageSize,
                    TotalItems = totalCount,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paginated books.");
                throw;
            }
        }


        public static Expression<Func<Book, bool>> FilterBy(string filterBy, string value)
        {
            if (string.IsNullOrWhiteSpace(filterBy)) return b => true;
            return filterBy switch
            {
                "Title" => b => b.Title.Contains(value),
                "Description" => b => b.Description.Contains(value),
                "ISBN" => b => b.ISBN.Contains(value),
                "Authors" => b => b.Authors.Any(x => x.FullName.Contains(value)),
                "Categories" => b => b.Categories.Any(x => x.FullName.Contains(value)),
                "Publisher" => b => b.Publisher.FullName.Contains(value),
                _ => b => false
            };
        }
    }
}
