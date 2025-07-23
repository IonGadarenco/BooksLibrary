using AutoMapper;
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.App.Books.Exceptions;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.Application.Common.Abstractions;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BooksLibrary.Application.App.Books.Commands
{
    public class CreateBookCommand : IRequest<PublicBookDetailsDto>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public IFormFile CoverImageFile { get; set; }
        public string? CoverImageUrl { get; set; }
        public PublisherDto Publisher { get; set; }
        public List<AuthorDto> Authors { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
    public class CreateBookHandler : IRequestHandler<CreateBookCommand, PublicBookDetailsDto>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Author> _authorRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Publisher> _publisherRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateBookHandler> _logger;
        private readonly IAzureBlobService _azureBlob;

        public CreateBookHandler(
            IRepository<Book> bookRepository,
            IRepository<Author> authorRepository,
            IRepository<Category> categoryRepository,
            IRepository<Publisher> publisherRepository,
            IMapper mapper,
            ILogger<CreateBookHandler> logger,
            IAzureBlobService azureBlobService)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _publisherRepository = publisherRepository;
            _mapper = mapper;
            _logger = logger;
            _azureBlob = azureBlobService;
        }
        public async Task<PublicBookDetailsDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            if (request.CoverImageFile != null && request.CoverImageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.CoverImageFile.FileName)}";
                var containerName = "bookscoverimg";
                await using var stream = request.CoverImageFile.OpenReadStream();
                var uri = await _azureBlob.UploadFileAsync(stream, fileName, containerName);
                request.CoverImageUrl = uri.ToString();
                _logger.LogInformation("Uploaded cover image to {Url}", request.CoverImageUrl);
            }

          
            var authors = new List<Author>();
            foreach (var a in request.Authors)
            {
                var author = await _authorRepository.GetByNameAsync(a.FullName)
                    ?? new Author
                    {
                        FullName = a.FullName,
                    };
                author = await _authorRepository.AddAsync(author);

                authors.Add(author);
            }

            var categories = new List<Category>();
            foreach (var c in request.Categories)
            {
                var category = await _categoryRepository.GetByNameAsync(c.FullName)
                    ?? new Category
                    {
                        FullName = c.FullName
                    }; 
                category = await _categoryRepository.AddAsync(category); ;
  
                categories.Add(category);
            }

            var publisher = await _publisherRepository.GetByNameAsync(request.Publisher.FullName)
                ?? new Publisher
                {
                    FullName = request.Publisher.FullName,
                    Address = request.Publisher.Address
                };
            publisher = await _publisherRepository.AddAsync(publisher);
            await _publisherRepository.SaveChangesAsync(); ;
            
            var existingBook = await _bookRepository.GetByTitleAndIsbnAsync(request.Title, request.ISBN);

            if (existingBook == null)
            {
                var isbnExist = await _bookRepository.DoesIsbnExist(request.ISBN);

                if(!isbnExist)
                {
                    throw new BookIsbnAlreadyExistException(request.ISBN);
                }

                var book = new Book
                {
                    Title = request.Title,
                    Description = request.Description,
                    ISBN = request.ISBN,
                    TotalCopies = request.TotalCopies,
                    PublisherId = publisher.Id,
                    Authors = authors,
                    Categories = categories,
                    CoverImageUrl = request.CoverImageUrl,
                };

                await _bookRepository.AddAsync(book);
                await _bookRepository.SaveChangesAsync();
                _logger.LogInformation("A new book '{Title}' was created successfully.", request.Title);

                return _mapper.Map<PublicBookDetailsDto>(book);
            }

            existingBook.TotalCopies++;
            await _bookRepository.UpdateAsync(existingBook);
            await _bookRepository.SaveChangesAsync();
            _logger.LogInformation("The book '{Title}' already exists. Incremented total copies.", request.Title);

            return _mapper.Map<PublicBookDetailsDto>(existingBook);
        }
    }
}
