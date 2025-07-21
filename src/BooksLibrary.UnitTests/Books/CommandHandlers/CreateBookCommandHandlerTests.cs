
using AutoMapper;
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.App.Books.Exceptions;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;


public class CreateBookHandlerTests
{
    private readonly Mock<IRepository<Book>> _bookRepoMock = new();
    private readonly Mock<IRepository<Author>> _authorRepoMock = new();
    private readonly Mock<IRepository<Category>> _categoryRepoMock = new();
    private readonly Mock<IRepository<Publisher>> _publisherRepoMock = new();
    private readonly Mock<ILogger<CreateBookHandler>> _loggerMock = new();
    private readonly IMapper _mapper;

    public CreateBookHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Book, PublicBookDetailsDto>();
        });
        _mapper = config.CreateMapper();
    }

    private CreateBookHandler CreateHandler() => new(
        _bookRepoMock.Object,
        _authorRepoMock.Object,
        _categoryRepoMock.Object,
        _publisherRepoMock.Object,
        _mapper,
        _loggerMock.Object);

    private CreateBookCommand GetValidCommand() => new()
    {
        Title = "Test Book",
        ISBN = "1234567890",
        Description = "Desc",
        TotalCopies = 1,
        Authors = new List<AuthorDto>() { new() { FullName = "Author 1" } },
        Categories = new List<CategoryDto>() { new() { FullName = "Category 1" } },
        Publisher = new PublisherDto() { FullName = "Publisher 1", Address = "Address 1" }
    };

    [Fact]
    public async Task Handle_Should_CreateBook_When_BookNotExists()
    {
        var handler = CreateHandler();
        var command = GetValidCommand();

        _authorRepoMock.Setup(r => r.GetByNameAsync("Author 1")).ReturnsAsync((Author)null);
        _authorRepoMock.Setup(r => r.AddAsync(It.IsAny<Author>())).ReturnsAsync(new Author { Id = 1, FullName = "Author 1" });

        _categoryRepoMock.Setup(r => r.GetByNameAsync("Category 1")).ReturnsAsync((Category)null);
        _categoryRepoMock.Setup(r => r.AddAsync(It.IsAny<Category>())).ReturnsAsync(new Category { Id = 1, FullName = "Category 1" });

        _publisherRepoMock.Setup(r => r.GetByNameAsync("Publisher 1")).ReturnsAsync((Publisher)null);
        _publisherRepoMock.Setup(r => r.AddAsync(It.IsAny<Publisher>())).ReturnsAsync(new Publisher { Id = 1, FullName = "Publisher 1", Address = "Address 1" });

        _bookRepoMock.Setup(r => r.GetByTitleAndIsbnAsync(command.Title, command.ISBN)).ReturnsAsync((Book)null);
        _bookRepoMock.Setup(r => r.CheckIsbn(command.ISBN)).ReturnsAsync(true);
        _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>())).ReturnsAsync(new Book { Id = 1, Title = command.Title });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Test Book", result.Title);
        _bookRepoMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("A new book") && v.ToString().Contains("Test Book")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_IsbnExists()
    {
        var handler = CreateHandler();
        var command = GetValidCommand();

        _bookRepoMock.Setup(r => r.GetByTitleAndIsbnAsync(command.Title, command.ISBN)).ReturnsAsync((Book)null);
        _bookRepoMock.Setup(r => r.CheckIsbn(command.ISBN)).ReturnsAsync(false);

        var result = handler.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<BookIsbnAlreadyExistException>(() => result);
    }

    [Fact]
    public async Task Handle_Should_UpdateTotalCopies_When_BookExists()
    {
        //Arrange
        var handler = CreateHandler();
        var command = GetValidCommand();

        var existingBook = new Book { Id = 1, Title = command.Title, ISBN = command.ISBN, TotalCopies = 1 };
        _bookRepoMock.Setup(r => r.GetByTitleAndIsbnAsync(command.Title, command.ISBN)).ReturnsAsync(existingBook);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.Equal(2, existingBook.TotalCopies);
        _bookRepoMock.Verify(r => r.UpdateAsync(It.Is<Book>(b => b.TotalCopies == 2)), Times.Once);
        Assert.NotNull(result);  // Poți verifica și conținutul lui result
        Assert.Equal(command.Title, result.Title);

    }

    [Fact]
    public async Task Handle_Should_AddAuthor_If_NotExists()
    {
        var handler = CreateHandler();
        var command = GetValidCommand();

        _authorRepoMock.Setup(r => r.GetByNameAsync("Author 1")).ReturnsAsync((Author)null);
        _authorRepoMock.Setup(r => r.AddAsync(It.IsAny<Author>())).ReturnsAsync(new Author { Id = 1, FullName = "Author 1" });

        _categoryRepoMock.Setup(r => r.GetByNameAsync("Category 1")).ReturnsAsync(new Category { Id = 1, FullName = "Category 1" });

        _publisherRepoMock.Setup(r => r.GetByNameAsync("Publisher 1")).ReturnsAsync(new Publisher { Id = 1, FullName = "Publisher 1" });

        _bookRepoMock.Setup(r => r.GetByTitleAndIsbnAsync(command.Title, command.ISBN)).ReturnsAsync((Book)null);
        _bookRepoMock.Setup(r => r.CheckIsbn(command.ISBN)).ReturnsAsync(true);
        _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>())).ReturnsAsync(new Book { Id = 1 });

        var result = await handler.Handle(command, CancellationToken.None);

        _authorRepoMock.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_ExceptionOccurs()
    {
        // Arrange
        var handler = CreateHandler();
        var command = GetValidCommand();

        var exceptionMessage = "Simulated repository failure";
        _authorRepoMock
            .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            handler.Handle(command, CancellationToken.None));

        Assert.Equal(exceptionMessage, exception.Message);
    }


}

