using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BooksLibrary.Application.App.Books.Commands
{
    public class DeleteBookCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
    public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, Unit>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly ILogger<DeleteBookHandler> _logger;
        public DeleteBookHandler(IRepository<Book> bookRepository, ILogger<DeleteBookHandler> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }
        public async Task<Unit> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id);

            if (book == null)
            {
                throw new EntityNotExistException("Book", request.Id);
            }

            await _bookRepository.RemoveAsync(book);
            await _bookRepository.SaveChangesAsync();
            _logger.LogInformation("Book with ID {Id} was deleted successfully", request.Id);
            return Unit.Value;
        }
    }
}
