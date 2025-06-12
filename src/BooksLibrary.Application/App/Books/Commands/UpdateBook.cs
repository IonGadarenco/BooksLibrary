
using AutoMapper;
using BooksLibrary.Application.App.Books.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using MediatR;

namespace BooksLibrary.Application.App.Books.Commands
{
    public class UpdateBook : IRequest<BookDto>
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ISBN { get; set; } = default!;
        public int TotalCopies { get; set; }
        public int PublisherId { get; set; }
    }
    public class UpdateBookHandler : IRequestHandler<UpdateBook, BookDto>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        public UpdateBookHandler(IRepository<Book> bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }
        public async Task<BookDto> Handle(UpdateBook request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id);

            if (book is null)
            {
                throw new EntityNotExistException("Book", request.Id);
            }

            book.Title = request.Title;
            book.Description = request.Description;
            book.ISBN = request.ISBN;
            book.TotalCopies = request.TotalCopies;
            book.PublisherId = request.PublisherId;

            var updatedBook = await _bookRepository.UpdateAsync(book);

            return _mapper.Map<BookDto>(updatedBook);
        }
    }
}
