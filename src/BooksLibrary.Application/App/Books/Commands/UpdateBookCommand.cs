
using AutoMapper;
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BooksLibrary.Application.App.Books.Commands
{
    public class UpdateBookCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PublisherDto Publisher { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
        public string CoverImageUrl { get; set; }
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }
    public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, Unit>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Author> _authorRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Publisher> _publisherRepository;
        private readonly ILogger<UpdateBookHandler> _logger;

        public UpdateBookHandler(
            IRepository<Book> bookRepository,
            IRepository<Author> authorRepository,
            IRepository<Category> categoryRepository,
            IRepository<Publisher> publisherRepository,
            ILogger<UpdateBookHandler> logger)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _publisherRepository = publisherRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            UpdateBookCommand request,
            CancellationToken cancellationToken)
        {
            // 1) Încarcă entitatea Book cu Authors, Categories şi Publisher
            var book = await _bookRepository.GetByIdAsync(request.Id);
            if (book == null)
                throw new EntityNotExistException("Book", request.Id);

            // 2) Actualizează proprietăţile simple
            book.Title = request.Title;
            book.Description = request.Description;
            book.ISBN = request.ISBN;
            book.TotalCopies = request.TotalCopies;
            book.CoverImageUrl = request.CoverImageUrl;

            // 3) Actualizează Publisher (1–1)
            //    Caută publisher existent după name sau îl creez
            var pub = await _publisherRepository
                            .GetByNameAsync(request.Publisher.FullName)
                      ?? new Publisher
                      {
                          FullName = request.Publisher.FullName,
                          Address = request.Publisher.Address
                      };
            book.Publisher = pub;

            // 4) Actualizează Authors (many-to-many)
            book.Authors.Clear();
            foreach (var authorDto in request.Authors)
            {
                var author = await _authorRepository
                                     .GetByNameAsync(authorDto.FullName)
                             ?? new Author { FullName = authorDto.FullName };
                book.Authors.Add(author);
            }

            // 5) Actualizează Categories (many-to-many)
            book.Categories.Clear();
            foreach (var categoryDto in request.Categories)
            {
                var category = await _categoryRepository
                                       .GetByNameAsync(categoryDto.FullName)
                               ?? new Category { FullName = categoryDto.FullName };
                book.Categories.Add(category);
            }

            // 6) Salvează modificările
            try
            {
                await _bookRepository.SaveChangesAsync();
                _logger.LogInformation(
                    "Book (ID: {Id}) was updated successfully.",
                    book.Id);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(
                  dbEx,
                  "Error saving Book ID {Id}: {Err}",
                  book.Id,
                  dbEx.InnerException?.Message ?? dbEx.Message);
                throw;
            }

            return Unit.Value;
        }
    }
}
