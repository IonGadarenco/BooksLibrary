using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

public class DeleteBookCommandHandlerTests
{
    private readonly Mock<IRepository<Book>> _bookRepoMock = new();
    private readonly Mock<ILogger<DeleteBookHandler>> _loggerMock = new();

    [Fact]
    public async Task Handle_BookExists_DeletesBookAndLogsSuccess()
    {
        //Arrange
        var command = new DeleteBookCommand { Id = 1 };
        var handler = new DeleteBookHandler(_bookRepoMock.Object, _loggerMock.Object);

        var book = new Book { Id = 1, Title = "Title 1" };
        _bookRepoMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync(book);
        _bookRepoMock.Setup(r => r.RemoveAsync(book)).Returns(Task.CompletedTask);

        //Act
        var result = handler.Handle(command, CancellationToken.None);

        //Assert
        _bookRepoMock.Verify(r => r.RemoveAsync(book), Times.Once);
    }

    [Fact]
    public async Task Handle_BookDoesNotExist_ThrowsEntityNotExistException()
    {
        // Arrange
        var command = new DeleteBookCommand { Id = 9 };
        var handler = new DeleteBookHandler(_bookRepoMock.Object, _loggerMock.Object);

        _bookRepoMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((Book)null);

        // Act
        var result = handler.Handle(command, CancellationToken.None);

        //Assert
        var exception = await Assert.ThrowsAsync<EntityNotExistException>(
            () => result);

        Assert.Equal("Entity Book with id 9 does not exist", exception.Message);
    }


}

