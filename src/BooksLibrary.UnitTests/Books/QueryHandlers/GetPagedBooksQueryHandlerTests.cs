using AutoMapper;
using BooksLibrary.Application.App.Books.DTOs;
using BooksLibrary.Application.App.Books.Queries;
using BooksLibrary.Application.Common.Models;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

public class GetPagedBooksQueryHandlerTests
{
    private readonly Mock<IRepository<Book>> _bookRepositoryMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<GetPagedBooksHandler>> _loggerMock;

    public GetPagedBooksQueryHandlerTests()
    {
        _bookRepositoryMock = new Mock<IRepository<Book>>();
        _loggerMock = new Mock<ILogger<GetPagedBooksHandler>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Book, BookListDto>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfBookListDto_WhenBooksExist()
    {
        // Arrange
        var pagedRequest = new PagedRequest { PageIndex = 1, PageSize = 2 };
        var query = new GetPagedBooksQuery { PagedRequest = pagedRequest };

        var books = new List<Book>
        {
            new Book { Id = 1, Title = "Book 1", ISBN = "ISBN1", TotalCopies = 5, Description = "Desc 1" },
            new Book { Id = 2, Title = "Book 2", ISBN = "ISBN2", TotalCopies = 3, Description = "Desc 2" }
        };

        _bookRepositoryMock
            .Setup(repo => repo.GetAllAsyncPaged(pagedRequest.PageIndex, pagedRequest.PageSize))
            .ReturnsAsync(books);

        var handler = new GetPagedBooksHandler(_bookRepositoryMock.Object, _mapper, _loggerMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Collection(result,
            item =>
            {
                Assert.Equal(1, item.Id);
                Assert.Equal("Book 1", item.Title);
                Assert.Equal("ISBN1", item.ISBN);
                Assert.Equal(5, item.TotalCopies);
                Assert.Equal("Desc 1", item.Description);
            },
            item =>
            {
                Assert.Equal(2, item.Id);
                Assert.Equal("Book 2", item.Title);
                Assert.Equal("ISBN2", item.ISBN);
                Assert.Equal(3, item.TotalCopies);
                Assert.Equal("Desc 2", item.Description);
            });

        _bookRepositoryMock.Verify(repo => repo.GetAllAsyncPaged(pagedRequest.PageIndex, pagedRequest.PageSize), Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching paged books")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Successfully fetched")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowAndLogException_WhenRepositoryThrows()
    {
        // Arrange
        var pagedRequest = new PagedRequest { PageIndex = 1, PageSize = 2 };
        var query = new GetPagedBooksQuery { PagedRequest = pagedRequest };

        var exception = new System.Exception("Repository failure");

        _bookRepositoryMock
            .Setup(repo => repo.GetAllAsyncPaged(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(exception);

        var handler = new GetPagedBooksHandler(_bookRepositoryMock.Object, _mapper, _loggerMock.Object);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<System.Exception>(() => handler.Handle(query, CancellationToken.None));

        Assert.Equal("Repository failure", ex.Message);

        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while fetching paged books")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
