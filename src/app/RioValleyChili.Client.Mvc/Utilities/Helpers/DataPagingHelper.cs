using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public static class DataPagingHelper
    {
        static DataPagingHelper()
        {
            MaxPageSize = 100;
        }

        public static int MaxPageSize { get; set; }

        public static IQueryable<TInput> PageResults<TInput>(this IQueryable<TInput> input, int? pageSize = 100, int? skipCount = 0)
        {
            var take = Math.Min(pageSize ?? MaxPageSize, MaxPageSize);
            var skip = Math.Max(skipCount ?? 0, 0);
            return input.Skip(skip).Take(take);
        } 
        public static IEnumerable<TInput> PageResults<TInput>(this IEnumerable<TInput> input, int? pageSize = 100, int? skipCount = 0)
        {
            var take = Math.Min(pageSize ?? MaxPageSize, MaxPageSize);
            var skip = Math.Max(skipCount ?? 0, 0);
            return input.Skip(skip).Take(take);
        }

        public static Task<IQueryable<TInput>> PageResultsAsync<TInput>(this IQueryable<TInput> input, int? pageSize = 100, int? skipCount = 0)
        {
            return Task.Run(() => input.PageResults(pageSize, skipCount));
        }
    }
}