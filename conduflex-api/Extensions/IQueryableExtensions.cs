using AutoMapper;
using conduflex_api.DTOs;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq.Dynamic.Core;


namespace conduflex_api.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<ListResponse<TDTO>> FilterSortPaginate<TEntity, TDTO>(
            this IQueryable<TEntity> queryable,
            BaseFilter baseFilter,
            IMapper mapper,
            IActionContextAccessor actionContextAccessor) where TEntity : class, new()
        {
            queryable = queryable.Filter(baseFilter.Filters);

            var count = await queryable.CountAsync();

            var list = queryable.Sort(baseFilter.Sort).ToList();

            list = list.Paginate(baseFilter.Range);

            return new ListResponse<TDTO>(mapper.Map<List<TDTO>>(list), count);
        }

        public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> queryable, string filtersString) where TEntity : class, new()
        {
            if (String.IsNullOrEmpty(filtersString)) return queryable;

            // See comment on DTOs/Filters/BaseFilter.cs line 8
            var filters = JsonConvert.DeserializeObject<List<FilterValue>>(filtersString);
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    string query = $"q => q.{filter.Field} {filter.Operator} @0";

                    switch (filter.AlternativeOperator)
                    {
                        case AlternativeOperators.None:
                            break;

                        case AlternativeOperators.StringContains:
                            query = $"q => q.{filter.Field}.ToLower().Contains(@0.ToLower())";
                            break;

                        case AlternativeOperators.DateTime:
                            query = $"q => q.{filter.Field}.Date {filter.Operator} DateTimeOffset.Parse(@0).UtcDateTime.Date";
                            break;

                        default:
                            break;
                    }

                    try
                    {
                        queryable = queryable.Where(query, filter.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error on filter {filter.Field} {filter.Operator} {filter.Value}", ex);
                    }
                }
            }
            return queryable;
        }

        public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> queryable, Sort sort)
        {
            if (sort == null) return queryable;
            var order = sort.IsAscending ? "" : "descending";
            return queryable.OrderBy($"{sort.Field} {order}");
        }

        public static List<TEntity> Paginate<TEntity>(this List<TEntity> list, conduflex_api.DTOs.Range range)
        {
            return list.Skip(range.Start).Take(range.End - range.Start + 1).ToList();
        }
    }
}
