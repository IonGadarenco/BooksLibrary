using BooksLibrary.API;
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using MediatR;

namespace BooksLibrary.IntegrationTests.Controllers
{
    public class BooksControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        [Fact]
        public async Task CreateBookHandler_Should_CreateBook_When_BookDoesNotExist()
        {
            var mediator = TestHelpers.CreateMediator();

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

            var result = await mediator.Send(command);

            Assert.NotNull(result);
            Assert.Equal(command.Title, result.Title);
        }
    }
}
