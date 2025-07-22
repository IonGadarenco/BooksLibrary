using BooksLibrary.Application.App.Authors.DTOs;

namespace BooksLibrary.Application.App.Likes.DTOs
{
    public class LikedBookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public List<AuthorDto> Authors { get; set; } = new();
        public string CoverImageUrl { get; set; } = default!;
    }
}
