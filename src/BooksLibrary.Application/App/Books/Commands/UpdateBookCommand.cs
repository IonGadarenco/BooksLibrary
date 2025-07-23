
using AutoMapper;
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.Application.Common.Abstractions;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        public IFormFile? NewCoverImageFile { get; set; }
        public string? OldCoverImageUrl { get; set; }
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }
    public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, Unit>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Author> _authorRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Publisher> _publisherRepository;
        private readonly IAzureBlobService _azureBlobService;
        private readonly ILogger<UpdateBookHandler> _logger;

        public UpdateBookHandler(
            IRepository<Book> bookRepository,
            IRepository<Author> authorRepository,
            IRepository<Category> categoryRepository,
            IRepository<Publisher> publisherRepository,
            ILogger<UpdateBookHandler> logger,
            IAzureBlobService azureBlobService)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _publisherRepository = publisherRepository;
            _logger = logger;
            _azureBlobService = azureBlobService;
        }

        public async Task<Unit> Handle(
            UpdateBookCommand request,
            CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id);
            if (book == null)
                throw new EntityNotExistException("Book", request.Id);

            if (!string.IsNullOrWhiteSpace(request.OldCoverImageUrl) && request.NewCoverImageFile?.Length > 0)
            {
                var (fileName, containerName) = _azureBlobService.GetBlobInfoFromUrl(request.OldCoverImageUrl);
                if (fileName != null) await _azureBlobService.DeleteBlobAsync(containerName, fileName);
            }

            if (request.NewCoverImageFile?.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{ Path.GetExtension(request.NewCoverImageFile.FileName)}";
                await using var streem = request.NewCoverImageFile.OpenReadStream();
                var uri = (await _azureBlobService.UploadFileAsync(streem, fileName, "bookscoverimg"));
                book.CoverImageUrl = uri.ToString();
            }

            book.Title = request.Title;
            book.Description = request.Description;
            book.ISBN = request.ISBN;
            book.TotalCopies = request.TotalCopies;

            var publisher = await _publisherRepository.GetByNameAsync(request.Publisher.FullName) 
                ?? new Publisher
                {
                    FullName = request.Publisher.FullName,
                    Address = request.Publisher.Address
                };
            book.Publisher = publisher;

            book.Authors.Clear();
            foreach (var authorDto in request.Authors)
            {
                var author = await _authorRepository.GetByNameAsync(authorDto.FullName)
                    ?? new Author { FullName = authorDto.FullName };
                book.Authors.Add(author);
            }

            book.Categories.Clear();
            foreach (var categoryDto in request.Categories)
            {
                var category = await _categoryRepository.GetByNameAsync(categoryDto.FullName)
                    ?? new Category { FullName = categoryDto.FullName };
                book.Categories.Add(category);
            }

            await _bookRepository.UpdateAsync(book);
            await _bookRepository.SaveChangesAsync();

            _logger.LogInformation("Book (ID: {Id}) was updated successfully.", book.Id);

            return Unit.Value;
        }
    }
}
