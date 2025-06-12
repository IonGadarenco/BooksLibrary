using AutoMapper;
using BooksLibrary.Application.App.Books.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;

namespace BooksLibrary.Application.App.Books.Queries
{
    public class GetPagedBooks : IRequest<List<BookListDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class GetPagedBooksHandler : IRequestHandler<GetPagedBooks, List<BookListDto>>
    {
        private readonly IRepository<Book> _bookRepository;
        public GetPagedBooksHandler(IRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public async Task<List<BookListDto>> Handle(GetPagedBooks request, CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetAllAsyncPaged(request.PageNumber, request.PageSize);
            var bookDtos = books.Select(b => new BookListDto
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                TotalCopies = b.TotalCopies,
                Description = b.Description,
            }).ToList();
            return bookDtos;
        }
    }
}
