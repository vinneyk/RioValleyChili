using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    [ExtractIntoSolutionheadLibrary]
    public class PagedDataResponse<TData>
    {
        public int CurrentPage { get; set; }
        public int TotalPageCount { get; set; }
        public IEnumerable<TData> Data { get; set; }
        
        public static PagedDataResponse<TData> BuildPagedDataResponse<TInput>(IQueryable<TInput> source, int pageSize, int skipCount)
        {
            var totalRecordCount = source.Count();

            return new PagedDataResponse<TData>
            {
                TotalPageCount = ComputeTotalPageCount(totalRecordCount, pageSize),
                CurrentPage = ComputeCurrentPage(skipCount, pageSize),
                Data = source
                    .PageResults(pageSize: pageSize, skipCount: skipCount)
                    .Project().To<TData>()
            };
        }

        private static int ComputeTotalPageCount(int totalRecordCount, int pageSize)
        {
            var prod = totalRecordCount/pageSize;
            var pageCount = totalRecordCount > 0 ? prod : 0;

            if (totalRecordCount%pageSize > 0)
            {
                pageCount++;
            }

            return pageCount;
        }

        private static int ComputeCurrentPage(int skipCount, int pageSize)
        {
            return (skipCount/pageSize) + 1;
        }
    }
}