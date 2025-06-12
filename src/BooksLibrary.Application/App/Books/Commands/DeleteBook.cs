using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using MediatR;

namespace BooksLibrary.Application.App.Books.Commands
{
    public record DeleteBook : IRequest
    {
        public int Id { get; set; }
    }
    public class DeleteBookHandler : IRequestHandler<DeleteBook>
    {
        private readonly IRepository<Book> _bookRepository;
        public DeleteBookHandler(IRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public async Task Handle(DeleteBook request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id);

            if (book == null)
            {
                throw new EntityNotExistException("Book", request.Id);
            }

            await _bookRepository.RemoveAsync(book);
        }
    }
}
