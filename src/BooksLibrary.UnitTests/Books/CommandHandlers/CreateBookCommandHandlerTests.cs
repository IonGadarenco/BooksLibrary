
using AutoMapper;
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.App.Books.Exceptions;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.Application.Common.Abstractions;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BooksLibrary.Application.Tests.App.Books.Commands
{
    public class CreateBookHandlerTests
    {
        private readonly Mock<IRepository<Book>> _bookRepoMock = new();
        private readonly Mock<IRepository<Author>> _authorRepoMock = new();
        private readonly Mock<IRepository<Category>> _categoryRepoMock = new();
        private readonly Mock<IRepository<Publisher>> _publisherRepoMock = new();
        private readonly Mock<ILogger<CreateBookHandler>> _loggerMock = new();
        private readonly Mock<IAzureBlobService> _azureBlobServiceMock = new();
        private readonly IMapper _mapper;
        private readonly IFormFile _dummyFile;

        public CreateBookHandlerTests()
        {
            // AutoMapper
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<Book, PublicBookDetailsDto>());
            _mapper = config.CreateMapper();

            // IFormFile dummy cu Length=0 (skip upload)
            var fileMock = new Mock<IFormFile>();
            fileMock.SetupGet(f => f.Length).Returns(0L);
            fileMock.Setup(f => f.OpenReadStream())
                    .Returns(new MemoryStream());
            fileMock.SetupGet(f => f.FileName).Returns("dummy.txt");
            fileMock.SetupGet(f => f.ContentType).Returns("text/plain");
            _dummyFile = fileMock.Object;

            // Stub UploadFileAsync (nu vom intra pe ramură)
            _azureBlobServiceMock
              .Setup(x => x.UploadFileAsync(
                  It.IsAny<Stream>(),
                  It.IsAny<string>(),
                  It.IsAny<string>()))
              .ReturnsAsync(new Uri("http://dummy.url"));
        }

        private CreateBookHandler CreateHandler() =>
            new CreateBookHandler(
                _bookRepoMock.Object,
                _authorRepoMock.Object,
                _categoryRepoMock.Object,
                _publisherRepoMock.Object,
                _mapper,
                _loggerMock.Object,
                _azureBlobServiceMock.Object
            );

        private CreateBookCommand GetValidCommand() => new()
        {
            Title = "Test Book",
            ISBN = "1234567890",
            Description = "Desc",
            TotalCopies = 1,
            CoverImageFile = _dummyFile,
            Publisher = new PublisherDto
            {
                FullName = "Publisher 1",
                Address = "Address 1"
            },
            Authors = new List<AuthorDto> { new() { FullName = "Author 1" } },
            Categories = new List<CategoryDto> { new() { FullName = "Category 1" } }
        };

        [Fact]
        public async Task Handle_Should_CreateBook_When_BookNotExists()
        {
            // Arrange
            var handler = CreateHandler();
            var cmd = GetValidCommand();

            // Autor inexistent → se adaugă
            _authorRepoMock
                .Setup(r => r.GetByNameAsync("Author 1"))
                .ReturnsAsync((Author)null);
            _authorRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Author>()))
                .ReturnsAsync(new Author { Id = 1, FullName = "Author 1" });

            // Categorie inexistentă → se adaugă
            _categoryRepoMock
                .Setup(r => r.GetByNameAsync("Category 1"))
                .ReturnsAsync((Category)null);
            _categoryRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Category>()))
                .ReturnsAsync(new Category { Id = 1, FullName = "Category 1" });

            // Publisher inexistent → se adaugă + SaveChanges
            _publisherRepoMock
                .Setup(r => r.GetByNameAsync("Publisher 1"))
                .ReturnsAsync((Publisher)null);
            _publisherRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Publisher>()))
                .ReturnsAsync(new Publisher
                {
                    Id = 1,
                    FullName = "Publisher 1",
                    Address = "Address 1"
                });
            _publisherRepoMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Cartea nu există → GetByTitleAndIsbnAsync = null
            _bookRepoMock
                .Setup(r => r.GetByTitleAndIsbnAsync(cmd.Title, cmd.ISBN))
                .ReturnsAsync((Book)null);

            // DoesIsbnExist = true (să nu arunce excepție)
            _bookRepoMock
                .Setup(r => r.DoesIsbnExist(cmd.ISBN))
                .ReturnsAsync(true);

            // Adăugare carte + SaveChanges
            _bookRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Book>()))
                .ReturnsAsync(new Book { Id = 1, Title = cmd.Title });
            _bookRepoMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Book", result.Title);

            _bookRepoMock.Verify(
                r => r.AddAsync(It.IsAny<Book>()),
                Times.Once);
            _bookRepoMock.Verify(
                r => r.SaveChangesAsync(),
                Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString().Contains("A new book") &&
                        v.ToString().Contains("Test Book")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_IsbnDoesNotExist()
        {
            // Arrange
            var handler = CreateHandler();
            var cmd = GetValidCommand();

            _bookRepoMock
                .Setup(r => r.GetByTitleAndIsbnAsync(cmd.Title, cmd.ISBN))
                .ReturnsAsync((Book)null);
            _bookRepoMock
                .Setup(r => r.DoesIsbnExist(cmd.ISBN))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<BookIsbnAlreadyExistException>(
                () => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_UpdateTotalCopies_When_BookExists()
        {
            // Arrange
            var handler = CreateHandler();
            var cmd = GetValidCommand();

            var existing = new Book
            {
                Id = 1,
                Title = cmd.Title,
                ISBN = cmd.ISBN,
                TotalCopies = 1
            };
            _bookRepoMock
                .Setup(r => r.GetByTitleAndIsbnAsync(cmd.Title, cmd.ISBN))
                .ReturnsAsync(existing);
            _bookRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<Book>()))
                .ReturnsAsync((Book b) => b);
            _bookRepoMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.Equal(2, existing.TotalCopies);
            _bookRepoMock.Verify(
                r => r.UpdateAsync(
                    It.Is<Book>(b => b.TotalCopies == 2)),
                Times.Once);
            Assert.Equal(cmd.Title, result.Title);
        }

        [Fact]
        public async Task Handle_Should_Throw_If_AuthorRepoFails()
        {
            // Arrange
            var handler = CreateHandler();
            var cmd = GetValidCommand();

            _authorRepoMock
                .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("fail"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(
                () => handler.Handle(cmd, CancellationToken.None));
            Assert.Equal("fail", ex.Message);
        }
    }
}