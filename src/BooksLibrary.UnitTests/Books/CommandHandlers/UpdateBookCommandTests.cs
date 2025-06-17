using AutoMapper;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.App.Books.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

public class UpdateBookCommandTests
{
    private readonly Mock<IRepository<Book>> _bookRepoMock = new();
    private readonly Mock<ILogger<UpdateBookHandler>> _loggerMock = new();
    private readonly IMapper _mapper;

    public UpdateBookCommandTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Book, UpdateBookCommandDto>();
            cfg.CreateMap<UpdateBookCommand, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ShouldUpdateBook_WhenBookExists()
    {
        // Arrange
        var command = new UpdateBookCommand
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Updated Description",
            PublisherId = 1
        };

        var existingBook = new Book { Id = 1 };

        _bookRepoMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync(existingBook);
        _bookRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Book>())).ReturnsAsync(existingBook);

        var handler = new UpdateBookHandler(_bookRepoMock.Object, _mapper, _loggerMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _bookRepoMock.Verify(r => r.UpdateAsync(It.Is<Book>(b => b.Title == command.Title && b.ISBN == command.ISBN)), Times.Once);
        Assert.IsType<UpdateBookCommandDto>(result);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenBookDoesNotExist()
    {
        // Arrange
        var command = new UpdateBookCommand { Id = 99 };

        _bookRepoMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((Book)null);

        var handler = new UpdateBookHandler(_bookRepoMock.Object, _mapper, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotExistException>(() => handler.Handle(command, CancellationToken.None));
    }
}

