using System.Collections.Generic;
using System.Linq;
using Solutionhead.Services;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class IResultExtensions
    {
        public static string CombineMessages(this IEnumerable<IResult> results)
        {
            return string.Join("\n", results.Select(r => r.Message).Where(m => !string.IsNullOrWhiteSpace(m)));
        }
    }
}