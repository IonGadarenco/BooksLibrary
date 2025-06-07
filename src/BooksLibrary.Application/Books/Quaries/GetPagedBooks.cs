using BooksLibrary.Application.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;

namespace BooksLibrary.Application.Books.Quaries
{
    public record GetPagedBooks(int pageNamber = 1, int pageSize = 10) : IRequest<List<Book>>;
    public class GetPagedBooksHandler : IRequestHandler<GetPagedBooks, List<Book>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetPagedBooksHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<List<Book>> Handle(GetPagedBooks request, CancellationToken cancellationToken)
        {
            var books = _unitOfWork.GetRepository<Book>().GetAllAsync(request.pageNamber, request.pageSize);
            return books;
        }
    }
}
