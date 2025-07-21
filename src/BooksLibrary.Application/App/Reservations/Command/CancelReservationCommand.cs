

using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary.Application.App.Reservations.Command
{
    public class CancelReservationCommand : IRequest<Unit>
    {
        public int BookId { get; set; }
    }

    public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, Unit>
    {
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CancelReservationCommandHandler(
            IRepository<Reservation> reservationRepository,
            IRepository<User> userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _reservationRepository = reservationRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var user = await _userRepository.GetByAuth0IdAsync(userIdClaim);
;            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var reservations = await _reservationRepository.GetAllAsync();
            var activeReservation = await reservations.FirstOrDefaultAsync(r =>
                r.BookId == request.BookId && r.UserId == user.Id && r.IsActive, cancellationToken);

            if (activeReservation == null)
            {
                throw new KeyNotFoundException("No active reservation found for this book.");
            }

            activeReservation.IsActive = false;

            await _reservationRepository.UpdateAsync(activeReservation);
            await _reservationRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
