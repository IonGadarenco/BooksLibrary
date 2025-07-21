

using BooksLibrary.Application.App.Books.Commands.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary.Application.App.Reviews.Command
{
    public class AddReviewCommand : IRequest<ReviewDto>
    {
        public int BookId { get; set; }
        public string Comment { get; set; }
    }

    public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, ReviewDto>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AddReviewCommandHandler(
            IRepository<Book> bookRepository, 
            IRepository<Review> reviewRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _bookRepository = bookRepository;
            _reviewRepository = reviewRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ReviewDto> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var user = await _reviewRepository.GetByAuth0IdAsync(userIdClaim);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            //var reviews = await _reviewRepository.GetAllAsync();
            //var existingReview = await reviews.AnyAsync(r => r.BookId == request.BookId && r.UserId == user.Id);
            //if (existingReview)
            //{
            //    throw new InvalidOperationException("You have already reviewed this book.");
            //}

            var newReview = new Review
            {
                BookId = request.BookId,
                UserId = user.Id,
                Comment = request.Comment
            };

            await _reviewRepository.AddAsync(newReview);
            await _reviewRepository.SaveChangesAsync();

            return new ReviewDto
            {
                Id = newReview.Id,
                Comment = newReview.Comment,
                UserName = $"{user.FirstName} {user.LastName}",
                CreatedAt = newReview.CreatedAt
            };
        }
    }
}
