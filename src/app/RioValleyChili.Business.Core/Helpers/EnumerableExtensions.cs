using System;
using System.Collections.Generic;
using System.Linq;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            var index = 0;
            foreach(var item in enumerable)
            {
                action(item, index++);
            }
        }

        public static IEnumerable<Tuple<T0, T1>> Stitched<T0, T1>(this IEnumerable<T0> a, IEnumerable<T1> b)
        {
            return a.Zip(b, (x, y) => new Tuple<T0, T1>(x, y));
        }

        public static List<T> Appended<T>(this List<T> source, params T[] append)
        {
            source.AddRange(append);
            return source;
        }

        public static List<T> ToAppendedList<T>(this IEnumerable<T> source, params T[] append)
        {
            var list = source as List<T> ?? source.ToList();
            list.AddRange(append);
            return list;
        }

        public static List<T> ToAppendedList<T>(this IEnumerable<T> source, IEnumerable<T> append)
        {
            var list = source as List<T> ?? source.ToList();
            list.AddRange(append);
            return list;
        }

        public static List<T> ToListWithModifiedElement<T>(this IEnumerable<T> source, int index, Func<T, T> modify)
        {
            var list = source as List<T> ?? source.ToList();
            list[index] = modify(list[index]);
            return list;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }

        public static IEnumerable<TSelect> DistinctBySelect<TSource, TKey, TSelect>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TSelect> select)
        {
            return source.DistinctBy(keySelector).Select(select);
        }

        /// <summary>
        /// Orders potentialMatches according to the specified criteria, with the highest ranked match first.
        /// </summary>
        /// <param name="bachelor">The object that the potentialMatches will attempt to match.</param>
        /// <param name="potentialMatches">The collection of items that will attempt to match to the bachelor object.</param>
        /// <param name="matchCriteria">Variable number of predicates that will increase a match's rank by a factor of 1 for each one that evaluates to true.</param>
        /// <returns>A collection of RankedMatch objects wrapping around the potentialMatches, ordered by the highest ranked match first.</returns>
        public static IEnumerable<RankedMatch<T1>> RankMatches<T0, T1>(this T0 bachelor, IEnumerable<T1> potentialMatches, params Func<T0, T1, bool>[] matchCriteria)
            where T0 : class
        {
            if(bachelor == null) { throw new ArgumentNullException("bachelor"); }
            if(potentialMatches == null) { throw new ArgumentNullException("potentialMatches"); }
            if(matchCriteria == null) { throw new ArgumentNullException("matchCriteria"); }

            return potentialMatches.Select(potential => new RankedMatch<T1>(potential, matchCriteria.Count(c => c(bachelor, potential))))
                .OrderByDescending(m => m.Rank);
        }

        /// <summary>
        /// Matches this enumerable up with the supplied potential matches according to the specified criteria.
        /// </summary>
        /// <param name="bachelors">This collection will attempt to find a match with members of the potentials collection first.</param>
        /// <param name="potentials">A collection of objects that are potential matches for any other object in the bachelors collection.</param>
        /// <param name="matchCriteria">Variable number of predicates that will increase a match's rank by a factor of 1 for each one that evaluates to true.</param>
        /// <returns>A Tuple representing the best matches made between objects in this collection and the supplied potentials. All objects in this collection and all objects in the potentials collection will be represented exactly once, with either's counterpart potentially being null if no match was made.</returns>
        public static IEnumerable<Tuple<T0, T1>> BestMatches<T0, T1>(this IEnumerable<T0> bachelors, IEnumerable<T1> potentials, params Func<T0, T1, bool>[] matchCriteria)
            where T0 : class
            where T1 : class
        {
            if(bachelors == null) { throw new ArgumentNullException("bachelors"); }
            if(potentials == null) { throw new ArgumentNullException("potentials"); }
            if(matchCriteria == null) { throw new ArgumentNullException("matchCriteria"); }

            var bachelorsList = bachelors.ToList();
            if(!bachelorsList.Any())
            {
                return potentials.Select(p => new Tuple<T0, T1>(null, p));
            }

            var potentialsList = potentials.ToList();
            if(!potentialsList.Any())
            {
                return bachelorsList.Select(b => new Tuple<T0, T1>(b, null));
            }

            var bachelorCandidates = bachelorsList.Select(b => new ItemWithRankedMatches<T0, T1>(b, b.RankMatches(potentials, matchCriteria).ToList()))
                .OrderByDescending(b => b.RankedMatches.Max(m => m.Rank)).ThenBy(b => b.RankedMatches.Sum(m => m.Rank)).ToList();

            var potentialCandidates = potentialsList.Select(p => new ItemWithRankedMatches<T1, ItemWithRankedMatches<T0, T1>>(p,
                bachelorCandidates.Select(b => new RankedMatch<ItemWithRankedMatches<T0, T1>>(b, b.RankedMatches.Single(m => m.Match == p).Rank)).OrderByDescending(m => m.Rank).ThenBy(m => m.Match.RankedMatches.Sum(bm => bm.Rank)).ToList()
                )).ToList();

            var matches = new List<Tuple<T0, T1>>();
            foreach(var bachelor in bachelorCandidates)
            {
                var match = bachelor.RankedMatches.FirstOrDefault(bm =>
                    {
                        var potential = potentialCandidates.Single(p => p.Item == bm.Match);
                        return !potential.RankedMatches.Any() || potential.RankedMatches.First().Match == bachelor;
                    });
                if(match != null)
                {
                    matches.Add(new Tuple<T0, T1>(bachelor.Item, match.Match));
                    bachelor.RankedMatches.Clear();
                    bachelorCandidates.ForEach(b => b.RankedMatches.RemoveAll(m => m.Match == match.Match));
                    potentialCandidates.RemoveAll(p => p.Item == match.Match);
                    potentialCandidates.ForEach(p => p.RankedMatches.RemoveAll(m => m.Match == bachelor));
                }
            }

            matches.AddRange(bachelorCandidates.Where(b => matches.All(m => m.Item1 != b.Item)).Select(b => new Tuple<T0, T1>(b.Item, null)));
            matches.AddRange(potentialCandidates.Where(p => matches.All(m => m.Item2 != p.Item)).Select(p => new Tuple<T0, T1>(null, p.Item)));

            return matches;
        }

        public class RankedMatch<T0>
        {
            public T0 Match { get; private set; }

            public int Rank { get; private set; }

            internal RankedMatch(T0 match, int rank)
            {
                Match = match;
                Rank = rank;
            }
        }

        public class ItemWithRankedMatches<T0, T1>
        {
            public T0 Item { get; private set; }

            public List<RankedMatch<T1>> RankedMatches { get; private set; }

            internal ItemWithRankedMatches(T0 item, List<RankedMatch<T1>> rankedMatches)
            {
                Item = item;
                RankedMatches = rankedMatches;
            }
        }

        public static ToBuilder<TSource> To<TSource>(this IEnumerable<TSource> source)
        {
            return new ToBuilder<TSource>(source);
        }

        public class ToBuilder<TSource>
        {
            private readonly IEnumerable<TSource> _source;

            public ToBuilder(IEnumerable<TSource> source)
            {
                _source = source;
            }

            public IDictionary<TKey, TValue> Dictionary<TKey, TValue>(Func<TSource, TKey> selectKey, Func<TSource, TValue> selectElement)
            {
                return _source.ToDictionary(selectKey, selectElement);
            }
        }
    }
}