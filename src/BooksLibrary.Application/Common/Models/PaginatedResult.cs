

namespace BooksLibrary.Application.Common.Models
{
    public class PaginatedResult<T>
    {
        public IList<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
