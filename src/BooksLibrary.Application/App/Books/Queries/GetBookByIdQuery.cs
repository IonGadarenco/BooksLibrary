using AutoMapper;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Application.Commun.Exceptions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BooksLibrary.Application.App.Books.Queries
{
    public class GetBookByIdQuery : IRequest<BookDetailsBaseDto>
    {
        public int Id { get; set; }
    }
    public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, BookDetailsBaseDto>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBookByIdHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetBookByIdHandler(
            IRepository<Book> bookRepository, 
            IMapper mapper, 
            ILogger<GetBookByIdHandler> logger, 
            IHttpContextAccessor httpContextAccessor
            )
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BookDetailsBaseDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("admin") ?? false;

            var query = (await _bookRepository.GetAllAsync())
                .Where(b => b.Id == request.Id)
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .Include(b => b.Publisher)
                .Include(b => b.Reviews).ThenInclude(r => r.User) 
                .Include(b => b.Loans).ThenInclude(l => l.User)     
                .Include(b => b.Likes).ThenInclude(l => l.User)
                .Include(b => b.Reservations).ThenInclude(r => r.User);
            
            var book = await query.FirstOrDefaultAsync(cancellationToken);

            if (book == null)
            {
                throw new EntityNotExistException("Book", request.Id);
            }

            _logger.LogInformation("Book with ID = {Id} exists", book.Id);

            BookDetailsBaseDto bookDto = isAdmin
                ? _mapper.Map<AdminBookDetailsDto>(book)
                : _mapper.Map<PublicBookDetailsDto>(book);

            var activeLoans = book.Loans.Where(l => l.ReturnedAt == null).ToList();

            var activeReservations = book.Reservations.Where(r => r.IsActive).ToList();

            bookDto.AvailableCopies = book.TotalCopies - activeLoans.Count - activeReservations.Count;
            bookDto.LikeCount = book.Likes.Count;

            if (!string.IsNullOrEmpty(userIdClaim))
            {
                bookDto.UserHasLiked = book.Likes.Any(l => l.User.Auth0Id == userIdClaim);
                bookDto.UserHasActiveReservation = activeReservations.Any(r => r.User.Auth0Id == userIdClaim);

                var userActiveLoan = activeLoans.FirstOrDefault(l => l.User.Auth0Id == userIdClaim);
                bookDto.UserHasActiveLoan = userActiveLoan != null;
                bookDto.ActiveLoanDueDate = userActiveLoan?.DueDate; 
            }

            return bookDto;
        }
    }
}
