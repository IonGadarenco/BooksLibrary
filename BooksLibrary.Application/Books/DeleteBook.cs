using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksLibrary.Application.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;

namespace BooksLibrary.Application.Books
{
    public record DeleteBook(int bookId) : IRequest;
    public class DeleteBookHandler : IRequestHandler<DeleteBook>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteBookHandler(IUnitOfWork unitOfWork)
        { 
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(DeleteBook request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.GetRepository<Book>().GetByIdAsync(request.bookId);

            if (book is null)
            {
                throw new Exception("Book not found");
            }

            await _unitOfWork.GetRepository<Book>().RemoveAsync(book);
            await _unitOfWork.SaveAsync();
        }
    }
}
