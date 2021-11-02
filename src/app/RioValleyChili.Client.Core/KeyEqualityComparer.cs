using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Core
{
    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        //todo migrate into Solutionhead.Core
        //based on the post at http://stackoverflow.com/a/3719802/538455

        protected readonly Func<T, TKey> KeyExtractor;

        public KeyEqualityComparer(Func<T, TKey> keyExtractor)
        {
            KeyExtractor = keyExtractor;
        }

        public virtual bool Equals(T x, T y)
        {
            return KeyExtractor(x).Equals(KeyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            return KeyExtractor(obj).GetHashCode();
        }
    }
}