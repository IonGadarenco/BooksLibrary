using BooksLibrary.API;
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.App.Books.Queries;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.Application.Common.Models;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.IntegrationTests.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BooksLibrary.IntegrationTests.Controllers
{
    public class BooksControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly IMediator _mediator;

        public BooksControllerTests()
        {
            _mediator = TestHelpers.CreateMediator();
        }

        [Fact]
        public async Task CreateBookHandler_Should_CreateBook_When_BookDoesNotExist()
        {
            var command = new CreateBookCommand
            {
                Title = "Test Book",
                Description = "Test Description",
                ISBN = "1234567890",
                TotalCopies = 1,
                Publisher = new PublisherDto { FullName = "Test Publisher", Address = "Test Address" },
                Authors = new List<AuthorDto> { new AuthorDto { FullName = "Author One" } },
                Categories = new List<CategoryDto> { new CategoryDto { FullName = "Category One" } }
            };

            var result = await _mediator.Send(command);

            Assert.NotNull(result);
            Assert.Equal(command.Title, result.Title);
        }

        [Fact]
        public async Task GetBookByIdHandler_Should_ReturnBook_When_Exists()
        {
            var createCommand = new CreateBookCommand
            {
                Title = "Book To Find",
                Description = "Description",
                ISBN = "1122334455",
                TotalCopies = 1,
                Publisher = new PublisherDto { FullName = "Pub", Address = "Address" },
                Authors = new List<AuthorDto> { new AuthorDto { FullName = "Author" } },
                Categories = new List<CategoryDto> { new CategoryDto { FullName = "Category" } }
            };

            var createdBook = await _mediator.Send(createCommand);

            var query = new GetBookByIdQuery { Id = createdBook.Id };
            var result = await _mediator.Send(query);

            Assert.NotNull(result);
            Assert.Equal(createdBook.Id, result.Id);
        }

        [Fact]
        public async Task UpdateBookHandler_Should_UpdateTitle_When_ValidId()
        {
            var createCommand = new CreateBookCommand
            {
                Title = "Original Title",
                Description = "Desc",
                ISBN = "998877",
                TotalCopies = 1,
                Publisher = new PublisherDto { FullName = "Pub", Address = "Addr" },
                Authors = new List<AuthorDto> { new AuthorDto { FullName = "Author" } },
                Categories = new List<CategoryDto> { new CategoryDto { FullName = "Cat" } }
            };

            var created = await _mediator.Send(createCommand);

            var updateCommand = new UpdateBookCommand
            {
                Id = created.Id,
                Title = "Updated Title",
                Description = created.Description,
                PublisherId = created.PublisherId
            };

            var updated = await _mediator.Send(updateCommand);

            Assert.NotNull(updated);
            Assert.Equal("Updated Title", updated.Title);
        }

        [Fact]
        public async Task DeleteBookHandler_Should_RemoveBook_When_ValidId()
        {
            var createCommand = new CreateBookCommand
            {
                Title = "Book To Delete",
                Description = "Desc",
                ISBN = "111222",
                TotalCopies = 1,
                Publisher = new PublisherDto { FullName = "Pub", Address = "Addr" },
                Authors = new List<AuthorDto> { new AuthorDto { FullName = "Author" } },
                Categories = new List<CategoryDto> { new CategoryDto { FullName = "Cat" } }
            };

            var created = await _mediator.Send(createCommand);

            await _mediator.Send(new DeleteBookCommand { Id = created.Id });

            await Assert.ThrowsAsync<EntityNotExistException>(() =>
            _mediator.Send(new GetBookByIdQuery { Id = created.Id }));
        }

        [Fact]
        public async Task GetPagedBooksHandler_Should_ReturnCorrectNumberOfBooks()
        {
            for (int i = 0; i < 15; i++)
            {
                await _mediator.Send(new CreateBookCommand
                {
                    Title = $"Book {i}",
                    Description = "Desc",
                    ISBN = $"{1000 + i}",
                    TotalCopies = 1,
                    Publisher = new PublisherDto { FullName = $"Pub{i}", Address = "Addr" },
                    Authors = new List<AuthorDto> { new AuthorDto { FullName = $"Author{i}" } },
                    Categories = new List<CategoryDto> { new CategoryDto { FullName = $"Cat{i}" } }
                });
            }

            var result = await _mediator.Send(new GetPagedBooksQuery
            {
                PagedRequest = new PagedRequest
                {
                    PageIndex = 2,
                    PageSize = 5
                }
            });

            Assert.NotNull(result);
            Assert.Equal(5, result.Items.Count());
            Assert.Equal("Book 5", result.Items[0].Title);
        }
    }
}
