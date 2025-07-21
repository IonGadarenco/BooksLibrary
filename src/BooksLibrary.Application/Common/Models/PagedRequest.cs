

namespace BooksLibrary.Application.Common.Models
{
    public class PagedRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string ColumnNameForSorting { get; set; }
        public string SortDirection { get; set; }
        //public RequestFilters RequestFilters { get; set; }
        public string SearchBy { get; set; }
        public string SearchValue { get; set; }
        //public OrderByType OrderBy { get; set; }
        //public bool OrderAsc { get; set; }
    }

    public enum FilterByType
    {
        Author,
        ISBN,
        Title
    }

    public enum OrderByType
    {
        Author,
        Title,
        Category
    }

}
