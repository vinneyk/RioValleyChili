using System;
using System.Collections.Generic;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ParsedParameteresExtensions
    {
        internal static IResult<List<TResult>> ToParsedParameters<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IResult<TResult>> toResult, bool emptyIfNull = true)
        {
            var results = new List<TResult>();
            foreach(var item in source == null ? (emptyIfNull ? new List<TSource>() : null) : source)
            {
                var itemResult = toResult(item);
                if(!itemResult.Success)
                {
                    return itemResult.ConvertTo<List<TResult>>();
                }

                results.Add(itemResult.ResultingObject);
            }

            return new SuccessResult<List<TResult>>(results);
        }
    }
}