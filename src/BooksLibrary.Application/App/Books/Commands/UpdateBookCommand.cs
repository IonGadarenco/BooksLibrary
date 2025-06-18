
using AutoMapper;
using BooksLibrary.Application.App.Books.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BooksLibrary.Application.App.Books.Commands
{
    public class UpdateBookCommand : IRequest<UpdateBookCommandDto>
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int PublisherId { get; set; }
    }
    public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, UpdateBookCommandDto>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateBookHandler> _logger;
        public UpdateBookHandler(IRepository<Book> bookRepository, IMapper mapper, ILogger<UpdateBookHandler> logger)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<UpdateBookCommandDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var book = await _bookRepository.GetByIdAsync(request.Id);

                if (book is null)
                {
                    throw new EntityNotExistException("Book", request.Id);
                }

                book.Title = request.Title;
                book.Description = request.Description;
                book.PublisherId = request.PublisherId;

                var updatedBook = await _bookRepository.UpdateAsync(book);

                _logger.LogInformation("Book '{Title}' (ID: {Id}) was updated successfully.", book.Title, book.Id);
                return _mapper.Map<UpdateBookCommandDto>(updatedBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the book with ID {Id}", request.Id);
                throw;
            }
        }
    }
}
