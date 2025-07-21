using AutoMapper;
using BooksLibrary.Application.App.Books.Commands.DTOs;
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
    public async Task Handle_ShouldThrowAndLogException_WhenRepositoryThrows()
    {
        // Arrange
        var pagedRequest = new PagedRequest { PageIndex = 0, PageSize = 2 };
        var query = new GetPagedBooksQuery { PagedRequest = pagedRequest };

        var exception = new Exception("Repository failure");

        _bookRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ThrowsAsync(exception);

        var handler = new GetPagedBooksHandler(_bookRepositoryMock.Object, _mapper, _loggerMock.Object);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));

        Assert.Equal("Repository failure", ex.Message);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while fetching paginated books")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
