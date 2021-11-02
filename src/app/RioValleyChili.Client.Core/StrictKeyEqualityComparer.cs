using System;

namespace RioValleyChili.Client.Core
{
    public class StrictKeyEqualityComparer<T, TKey> : KeyEqualityComparer<T, TKey>
        where TKey : IEquatable<TKey>
    {
        //todo migrate into Solutionhead.Core
        //based on the post at http://stackoverflow.com/a/3719802/538455

        public StrictKeyEqualityComparer(Func<T, TKey> keyExtractor)
            : base(keyExtractor)  { }

        public override bool Equals(T x, T y)
        {
            // This will use the overload that accepts a TKey parameter
            // instead of an object  parameter to avoid boxing.
            return KeyExtractor(x).Equals(KeyExtractor(y));
        }
    }
}