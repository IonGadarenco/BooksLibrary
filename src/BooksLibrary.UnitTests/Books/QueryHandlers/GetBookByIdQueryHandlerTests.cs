using AutoMapper;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.App.Books.Queries;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Application.Mappings;
using BooksLibrary.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;

public class GetBookByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Book>> _bookRepoMock;
    private readonly Mock<ILogger<GetBookByIdHandler>> _loggerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock; // 1. Adăugăm mock-ul nou
    private readonly IMapper _mapper;

    public GetBookByIdQueryHandlerTests()
    {
        _bookRepoMock = new Mock<IRepository<Book>>();
        _loggerMock = new Mock<ILogger<GetBookByIdHandler>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = config.CreateMapper();
    }

    // 2. Metodă ajutătoare pentru a simula rolul utilizatorului
    private void SetupHttpContextAccessor(string role)
    {
        var claims = new[] { new Claim(ClaimTypes.Role, role) };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
    }

    // 3. Actualizăm constructorul pentru a include noul mock
    private GetBookByIdHandler CreateHandler() =>
        new(_bookRepoMock.Object, _mapper, _loggerMock.Object, _httpContextAccessorMock.Object);

    [Fact]
    public async Task Handle_ShouldReturnPublicDto_WhenUserIsNotAdmin()
    {
        // Arrange
        SetupHttpContextAccessor("user"); // Simulăm un utilizator normal
        var handler = CreateHandler();

        var book = new Book { Id = 1, Title = "Test Book", TotalCopies = 5, Loans = new List<Loan>(), Reservations = new List<Reservation>() };
        var query = new GetBookByIdQuery { Id = 1 };

        // 4. Actualizăm setup-ul mock-ului pentru a se potrivi cu noua metodă
        _bookRepoMock.Setup(r => r.GetByIdAsync(query.Id, It.IsAny<Expression<Func<Book, object>>[]>()))
                     .ReturnsAsync(book);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PublicBookDetailsDto>(result); // Verificăm că primim DTO-ul public
        Assert.IsNotType<AdminBookDetailsDto>(result); // Ne asigurăm că NU este DTO-ul de admin
        Assert.Equal(book.Title, result.Title);
        _bookRepoMock.Verify(r => r.GetByIdAsync(query.Id, It.IsAny<Expression<Func<Book, object>>[]>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnAdminDto_WhenUserIsAdmin()
    {
        // Arrange
        SetupHttpContextAccessor("admin"); // Simulăm un admin
        var handler = CreateHandler();

        var book = new Book { Id = 1, Title = "Test Book", TotalCopies = 5, Loans = new List<Loan>(), Reservations = new List<Reservation>() };
        var query = new GetBookByIdQuery { Id = 1 };

        _bookRepoMock.Setup(r => r.GetByIdAsync(query.Id, It.IsAny<Expression<Func<Book, object>>[]>()))
                     .ReturnsAsync(book);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<AdminBookDetailsDto>(result); // Verificăm că primim DTO-ul de admin

        var adminResult = result as AdminBookDetailsDto;
        Assert.NotNull(adminResult.Loans); // Verificăm că proprietățile de admin sunt prezente
        Assert.NotNull(adminResult.Reservations);
        Assert.Equal(book.Title, adminResult.Title);
        _bookRepoMock.Verify(r => r.GetByIdAsync(query.Id, It.IsAny<Expression<Func<Book, object>>[]>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotExistException_WhenBookDoesNotExist()
    {
        // Arrange
        SetupHttpContextAccessor("user"); // Rolul nu contează aici, dar trebuie să fie prezent
        var handler = CreateHandler();
        var query = new GetBookByIdQuery { Id = 99 };

        _bookRepoMock.Setup(r => r.GetByIdAsync(query.Id, It.IsAny<Expression<Func<Book, object>>[]>()))
                     .ReturnsAsync((Book?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotExistException>(() => handler.Handle(query, CancellationToken.None));
    }
}