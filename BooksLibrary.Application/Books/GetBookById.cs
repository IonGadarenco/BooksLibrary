
using BooksLibrary.Application.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;

namespace BooksLibrary.Application.Books
{
    public record GetBookById(int bookId) : IRequest<Book>;
    public class GetBookByIdHandler : IRequestHandler<GetBookById, Book>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetBookByIdHandler(IUnitOfWork unitOfWork)
        { 
            _unitOfWork = unitOfWork;
        }
        public async Task<Book> Handle(GetBookById request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.GetRepository<Book>().GetByIdAsync(request.bookId);
        }
    }
}
