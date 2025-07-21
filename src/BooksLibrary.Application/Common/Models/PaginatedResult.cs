

namespace BooksLibrary.Application.Common.Models
{
    public class PaginatedResult<T>
    {
        public IList<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
