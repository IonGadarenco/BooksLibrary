using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Books.Commands;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.App.Books.Queries;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace BooksLibrary.IntegrationTests.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

        // Metodă ajutătoare pentru a simula rolul utilizatorului
        private void SetupHttpContextAccessor(string role)
        {
            var claims = new[] { new Claim(ClaimTypes.Role, role) };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        }

        // Metodă ajutătoare pentru a crea o comandă de test
        private CreateBookCommand CreateSampleBookCommand()
        {
            return new CreateBookCommand
            {
                Title = "Integration Test Book",
                Description = "A book for testing purposes",
                ISBN = $"IT-{Guid.NewGuid()}", // ISBN unic pentru fiecare test
                TotalCopies = 10,
                Publisher = new PublisherDto { FullName = "Test Publisher", Address = "123 Test St" },
                Authors = new List<AuthorDto> { new AuthorDto { FullName = "Test Author" } },
                Categories = new List<CategoryDto> { new CategoryDto { FullName = "Testing" } }
            };
        }

        [Fact]
        public async Task GetBookByIdHandler_Should_ReturnPublicDto_When_UserIsRegular()
        {
            // Arrange
            SetupHttpContextAccessor("user");
            var mediator = TestHelpers.CreateMediator(_httpContextAccessorMock);

            var createCommand = CreateSampleBookCommand();
            var createdBook = await mediator.Send(createCommand);

            var query = new GetBookByIdQuery { Id = createdBook.Id };

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PublicBookDetailsDto>(result);
            Assert.IsNotType<AdminBookDetailsDto>(result);
            Assert.Equal(createdBook.Id, result.Id);
        }

        [Fact]
        public async Task GetBookByIdHandler_Should_ReturnAdminDto_When_UserIsAdmin()
        {
            // Arrange
            SetupHttpContextAccessor("admin");
            var mediator = TestHelpers.CreateMediator(_httpContextAccessorMock);

            var createCommand = CreateSampleBookCommand();
            var createdBook = await mediator.Send(createCommand);

            var query = new GetBookByIdQuery { Id = createdBook.Id };

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AdminBookDetailsDto>(result);

            var adminResult = result as AdminBookDetailsDto;
            Assert.NotNull(adminResult.Loans);
            Assert.NotNull(adminResult.Reservations);
        }

        [Fact]
        public async Task DeleteBookHandler_Should_RemoveBook_When_ValidId()
        {
            // Arrange
            // Folosim O SINGURĂ instanță de mediator pentru tot testul
            SetupHttpContextAccessor("user");
            var mediator = TestHelpers.CreateMediator(_httpContextAccessorMock);

            var createCommand = CreateSampleBookCommand();
            var created = await mediator.Send(createCommand);

            // Act
            // Ștergem cartea folosind același mediator (și aceeași bază de date)
            await mediator.Send(new DeleteBookCommand { Id = created.Id });

            // Assert
            // Verificăm pe același mediator că entitatea nu mai există
            await Assert.ThrowsAsync<EntityNotExistException>(() =>
                mediator.Send(new GetBookByIdQuery { Id = created.Id }));
        }
    }
}