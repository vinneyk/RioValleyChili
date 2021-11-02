using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.Core.Helpers
{
    //todo: move into Solutionhead.Core library
    public class KeyEqualityComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
    {
        public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return x.Key.ToString() == y.Key.ToString();
        }

        public int GetHashCode(KeyValuePair<TKey, TValue> obj)
        {
            return obj.GetHashCode();
        }
    }
}