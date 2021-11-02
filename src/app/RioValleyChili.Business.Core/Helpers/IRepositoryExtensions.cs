using System.Collections.Generic;
using System.Linq;
using LinqKit;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class IRepositoryExtensions
    {
        public static IEnumerable<TResult> FindByKeys<TResult, TKey>(this IRepository<TResult> repository, out TKey[] notFound, params TKey[] keys)
            where TResult : class
            where TKey : IKey<TResult>
        {
            var predicate = (keys ?? new TKey[0]).Aggregate(PredicateBuilder.False<TResult>(), (c, n) => c.Or(n.FindByPredicate)).ExpandAll();
            var results = repository.Filter(predicate).ToList();
            notFound = (keys ?? new TKey[0]).Where(k => !results.Any(k.FindByPredicate.Compile())).ToArray();
            return results;
        }
    }
}