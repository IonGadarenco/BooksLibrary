//using AutoMapper;
//using AutoMapper.QueryableExtensions;
//using BooksLibrary.Application.Common.Models;
//using BooksLibrary.Domain.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Linq.Dynamic.Core;
//using System.Text;

//namespace BooksLibrary.Application.Extensions
//{
//    public static class QueryableExtensions
//    {
//        public static async Task<PaginatedResult<TDto>> CreatePaginatedResultAsync<TEntity, TDto>(
//            this IQueryable<TEntity> query,
//            PagedRequest pagedRequest,
//            IMapper mapper)
//            where TEntity : Entity
//            where TDto : class
//        {
//            var filteredQuery = query.ApplyFilters(pagedRequest);
//            var total = await filteredQuery.CountAsync();

//            var paginatedQuery = filteredQuery
//             .Sort(pagedRequest)
//             .Paginate(pagedRequest);

//            var listResult = await paginatedQuery
//             .ProjectTo<TDto>(mapper.ConfigurationProvider)
//             .ToListAsync();

//            return new PaginatedResult<TDto>
//            {
//                Items = listResult,
//                PageIndex = pagedRequest.PageIndex,
//                PageSize = pagedRequest.PageSize,
//                TotalItems = total,
//                TotalPages = (int)Math.Ceiling((double)total / pagedRequest.PageSize)
//            };
//        }



//        private static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, PagedRequest pagedRequest)
//        {
//            var predicate = new StringBuilder();
//            //var requestFilters = pagedRequest.RequestFilters;
//            var entityType = typeof(T);

//            if (requestFilters?.Filters == null || !requestFilters.Filters.Any())
//            {
//                return query;
//            }

//            for (int i = 0; i < requestFilters.Filters.Count; i++)
//            {
//                var filter = requestFilters.Filters[i];
//                var property = entityType.GetProperty(filter.Path);

//                if (i > 0)
//                {
//                    predicate.Append($" {requestFilters.LogicalOperator} ");
//                }

//                var propertyType = property.PropertyType;
//                if (propertyType == typeof(string))
//                {
//                    predicate.Append($"({filter.Path}.ToLower().Contains(@{i}))");
//                }

//                else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType)
//                         && propertyType != typeof(string))
//                {
//                    string innerProperty = "FullName"; 
//                    predicate.Append($"({filter.Path}.Any({innerProperty}.ToLower().Contains(@{i})))");
//                }

//                else if (propertyType.IsClass)
//                {
//                    string subProperty = "FullName";
//                    predicate.Append($"({filter.Path}.{subProperty}.ToLower().Contains(@{i}))");
//                }
//                else
//                {
//                    predicate.Append($"({filter.Path} == @{i})");
//                }
//            }

//            var propertyValues = requestFilters.Filters.Select(f => f.Value.ToLower()).ToArray();
//            return query.Where(predicate.ToString(), propertyValues);
//        }

//        private static IQueryable<T> Sort<T>(this IQueryable<T> query, PagedRequest request)
//        {
//            if (!string.IsNullOrWhiteSpace(request.ColumnNameForSorting))
//            {
//                return query.OrderBy($"{request.ColumnNameForSorting} {request.SortDirection}");
//            }

//            return query;
//        }

//        private static IQueryable<T> Paginate<T>(this IQueryable<T> query, PagedRequest request)
//        {
//            return query.Skip((request.PageIndex - 1) * request.PageSize)
//                        .Take(request.PageSize);
//        }
//    }
//}
