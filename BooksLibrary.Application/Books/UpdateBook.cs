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
    public record UpdateBook(int bookId) : IRequest<Book>;
    public class UpdateBookHandler : IRequestHandler<UpdateBook, Book>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateBookHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Book> Handle(UpdateBook request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.GetRepository<Book>().GetByIdAsync(request.bookId);

            if (book is null) 
            {
                throw new Exception("Book not existing");
            }

            return await _unitOfWork.GetRepository<Book>().UpdateAsync(book);
        }
    }
}
