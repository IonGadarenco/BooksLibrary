﻿

namespace BooksLibrary.Application.Common.Models
{
    public class PagedRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string ColumnNameForSorting { get; set; }
        public string SortDirection { get; set; }
        public RequestFilters RequestFilters { get; set; }
    }

}
