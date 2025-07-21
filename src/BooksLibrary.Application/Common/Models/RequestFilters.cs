

namespace BooksLibrary.Application.Common.Models
{
    public class RequestFilters
    {
        public FilterLogicalOperators LogicalOperator { get; set; }
        public IList<Filter> Filters { get; set; } 
    }

}
