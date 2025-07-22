using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Likes.DTOs;
using BooksLibrary.Application.Commun.Abstractions;
using BooksLibrary.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary.Application.App.Likes.Queries
{
    public class GetLikedBookQuery : IRequest<List<LikedBookDto>>
    {
    }

    public class GetLikedBookQueryHandler : IRequestHandler<GetLikedBookQuery, List<LikedBookDto>>
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetLikedBookQueryHandler(
            IRepository<Book> bookRepository,
            IHttpContextAccessor httpContextAccessor,
            IRepository<User> userRepository)
        {
            _bookRepository = bookRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }
        public async Task<List<LikedBookDto>> Handle(GetLikedBookQuery request, CancellationToken cancellationToken)
        {
            var auth0Id = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            var user = await _userRepository.GetByAuth0IdAsync(auth0Id);
            if (user == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var likedBooks = await _bookRepository.GetAllAsync()
                .Result.Where(b => b.Likes.Any(ul => ul.UserId == user.Id))
                .Include(b => b.Authors)
                .Select(b => new LikedBookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    CoverImageUrl = b.CoverImageUrl,
                    Authors = b.Authors.Select(a => new AuthorDto
                    {
                        FullName = a.FullName,
                    }).ToList()
                }).ToListAsync();

            return likedBooks;
        }
    }
}
