
using BooksLibrary.Application.App.Likes.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Entities;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary.Application.App.Likes.Command
{
    public class ToggleLikeCommand : IRequest<ToggleLikeResultDto>
    {
        public int BookId { get; set; }
    }
    public class ToggleLikeCommandHandler : IRequestHandler<ToggleLikeCommand, ToggleLikeResultDto>
    {
        private readonly IRepository<UserLike> _userLikeRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ToggleLikeCommandHandler(
            IRepository<UserLike> userLikeRepository,
            IRepository<User> userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userLikeRepository = userLikeRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ToggleLikeResultDto> Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
        {
            var auth0Id = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(auth0Id))
            {
                throw new UnauthorizedAccessException("User identifier not found.");
            }

            var user = await _userRepository.GetByAuth0IdAsync(auth0Id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found in local database.");
            }

            var allLikes = await _userLikeRepository.GetAllAsync();

            var existingLike = await allLikes.FirstOrDefaultAsync(ul =>
                ul.BookId == request.BookId && ul.UserId == user.Id);

            if (existingLike != null)
            {
                await _userLikeRepository.RemoveAsync(existingLike);
            }
            else
            {
                var newLike = new UserLike { BookId = request.BookId, UserId = user.Id };
                await _userLikeRepository.AddAsync(newLike);
            }

            await _userLikeRepository.SaveChangesAsync();

            var allLikesForBook = await _userLikeRepository.GetAllAsync();
            var newLikeCount = await allLikesForBook.CountAsync(ul => ul.BookId == request.BookId, cancellationToken);

            return new ToggleLikeResultDto
            {
                NewLikeCount = newLikeCount,
                UserHasLiked = existingLike == null
            };
        }
    }
}
