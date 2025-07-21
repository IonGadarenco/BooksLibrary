using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksLibrary.Application.App.Authors.DTOs;
using BooksLibrary.Application.App.Categories.DOTs;
using BooksLibrary.Application.App.Publishers.DTOs;

namespace BooksLibrary.Application.App.Books.Commands.DTOs
{
    public class BookDetailsBaseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public PublisherDto Publisher { get; set; }
        public string CoverImageUrl { get; set; }
        public List<AuthorDto> Authors { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public List<ReviewDto> Reviews { get; set; }
        public int AvailableCopies { get; set; }
        public int LikeCount { get; set; }
        public bool UserHasLiked { get; set; }
        public bool UserHasActiveReservation { get; set; } // NEW: Does the current user have a reservation?
        public bool UserHasActiveLoan { get; set; }      // NEW: Does the current user have this book loaned out?
        public DateTime? ActiveLoanDueDate { get; set; } // NEW: If so, when is it due?
    }
}
