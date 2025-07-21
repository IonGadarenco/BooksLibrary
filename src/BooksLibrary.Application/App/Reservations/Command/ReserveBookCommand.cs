

using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BooksLibrary.Application.App.Books.Commands.DTOs;
using AutoMapper;

namespace BooksLibrary.Application.App.Reservations.Command
{
    public class ReserveBookCommand : IRequest<Unit>
    {
        public int BookId { get; set; }
    }

    public class ReserveBookCommandHandler : IRequestHandler<ReserveBookCommand, Unit>
    {
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Book> _bookRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ReserveBookCommandHandler(
            IRepository<Reservation> reservationRepository,
            IRepository<User> userRepository,
            IRepository<Book> bookRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _reservationRepository = reservationRepository;
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(ReserveBookCommand request, CancellationToken cancellationToken)
        {
            var auth0Id = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(auth0Id))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var user = await _userRepository.GetByAuth0IdAsync(auth0Id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var reservations = await _reservationRepository.GetAllAsync();
            var hasActiveReservation = await reservations.AnyAsync(r =>
                r.BookId == request.BookId && r.UserId == user.Id && r.IsActive, cancellationToken);
            if (hasActiveReservation)
            {
                throw new InvalidOperationException("You already have an active reservation for this book.");
            }

            var books = await _bookRepository.GetAllAsync();
            var hasActiveLoan = await books
                .Where(b => b.Id == request.BookId)
                .SelectMany(b => b.Loans)
                .AnyAsync(l => l.UserId == user.Id && l.ReturnedAt == null, cancellationToken);

            if (hasActiveLoan)
            {
                throw new InvalidOperationException("You already have an active loan for this book.");
            }

            var newReservation = new Reservation
            {
                BookId = request.BookId,
                UserId = user.Id,
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };

            await _reservationRepository.AddAsync(newReservation);
            
            await _reservationRepository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
