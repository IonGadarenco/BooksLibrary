using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.Application.Common.Abstractions;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BooksLibrary.Application.Tests.App.Books.Commands
{
    public class UpdateBookCommandHandlerTests
    {
        private readonly Mock<IRepository<Book>> _bookRepoMock = new();
        private readonly Mock<IRepository<Author>> _authorRepoMock = new();
        private readonly Mock<IRepository<Category>> _categoryRepoMock = new();
        private readonly Mock<IRepository<Publisher>> _publisherRepoMock = new();
        private readonly Mock<ILogger<UpdateBookHandler>> _loggerMock = new();
        private readonly Mock<IAzureBlobService> _azureBlobServiceMock = new();

        private UpdateBookHandler CreateHandler() =>
            new UpdateBookHandler(
                _bookRepoMock.Object,
                _authorRepoMock.Object,
                _categoryRepoMock.Object,
                _publisherRepoMock.Object,
                _loggerMock.Object,
                _azureBlobServiceMock.Object
            );

        [Fact]
        public async Task Handle_ShouldUpdateBook_WhenBookExists()
        {
            // Arrange
            var cmd = new UpdateBookCommand
            {
                Id = 1,
                Title = "New Title",
                Description = "New Desc",
                ISBN = "ISBN123",
                TotalCopies = 10,
                // nu setăm OldCoverImageUrl/NewCoverImageFile → nu intră pe ramura de blob
                Publisher = new PublisherDto { FullName = "PubName", Address = "PubAddr" },
                Authors = new List<AuthorDto> { new() { FullName = "Author1" } },
                Categories = new List<CategoryDto> { new() { FullName = "Category1" } }
            };

            var existing = new Book
            {
                Id = 1,
                Title = "Old",
                Description = "OldDesc",
                ISBN = "OLDISBN",
                TotalCopies = 1,
                CoverImageUrl = null,
                Publisher = new Publisher { Id = 2, FullName = "OldPub", Address = "OldAddr" },
                Authors = new List<Author> { new() { Id = 3, FullName = "OldAuthor" } },
                Categories = new List<Category> { new() { Id = 4, FullName = "OldCat" } }
            };

            _bookRepoMock
                .Setup(r => r.GetByIdAsync(cmd.Id))
                .ReturnsAsync(existing);

            _publisherRepoMock
                .Setup(r => r.GetByNameAsync(cmd.Publisher.FullName))
                .ReturnsAsync((Publisher)null);

            _authorRepoMock
                .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Author)null);

            _categoryRepoMock
                .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Category)null);

            _bookRepoMock
                .Setup(r => r.UpdateAsync(existing))
                .ReturnsAsync(existing);
            _bookRepoMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);

            _bookRepoMock.Verify(r => r.UpdateAsync(
                It.Is<Book>(b =>
                    b.Id == cmd.Id &&
                    b.Title == cmd.Title &&
                    b.Description == cmd.Description &&
                    b.ISBN == cmd.ISBN &&
                    b.TotalCopies == cmd.TotalCopies
                )
            ), Times.Once);

            _bookRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            // Publisher-ul a fost înlocuit
            Assert.Equal("PubName", existing.Publisher.FullName);
            Assert.Equal("PubAddr", existing.Publisher.Address);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenBookDoesNotExist()
        {
            // Arrange
            var cmd = new UpdateBookCommand { Id = 99 };

            _bookRepoMock
                .Setup(r => r.GetByIdAsync(cmd.Id))
                .ReturnsAsync((Book)null);

            var handler = CreateHandler();

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotExistException>(
                () => handler.Handle(cmd, CancellationToken.None)
            );

            _bookRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
            _bookRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}