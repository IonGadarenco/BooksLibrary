using AutoMapper;
using BooksLibrary.Application.App.Books.Queries;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using BooksLibrary.Application.Mappings;
using Microsoft.Extensions.Logging;
using Moq;

public class GetBookByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Book>> _bookRepoMock = new();
    private readonly Mock<ILogger<GetBookByIdHandler>> _loggerMock = new();
    private readonly IMapper _mapper;

    public GetBookByIdQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = config.CreateMapper();
    }

    private GetBookByIdHandler CreateHandler() =>
        new(_bookRepoMock.Object, _mapper, _loggerMock.Object);

    [Fact]
    public async Task Handle_ShouldReturnBookDto_WhenBookExists()
    {
        // Arrange
        var handler = CreateHandler();
        var book = new Book { Id = 1, Title = "Test Book", ISBN = "123" };
        var query = new GetBookByIdQuery { Id = 1 };

        _bookRepoMock.Setup(r => r.GetByIdAsync(query.Id)).ReturnsAsync(book);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(book.Title, result.Title);
        Assert.Equal(book.ISBN, result.ISBN);
        _bookRepoMock.Verify(r => r.GetByIdAsync(query.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotExistException_WhenBookDoesNotExist()
    {
        // Arrange
        var handler = CreateHandler();
        var query = new GetBookByIdQuery { Id = 99 };

        _bookRepoMock.Setup(r => r.GetByIdAsync(query.Id)).ReturnsAsync((Book?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotExistException>(() => handler.Handle(query, CancellationToken.None));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("error occurred")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
