using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.Core
{
    [ExtractIntoSolutionheadLibrary]
    public static class CoreHelper
    {
        /// <summary>
        /// Creates a System.Collections.Dictionary<TKey, TValue> from a source IEnumerable<KeyValuePair<TKey, TValue>> object 
        /// using the implied Key and Value properties for the key and element selector functions.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source">A collection that contains the elements that will be added as elements to the new collection.</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            var dic = source.ToDictionary(d => d.Key, d => d.Value);
            return dic;
        }

        /// <summary>
        /// Creates a new dictionary object from the source object's properties.
        /// </summary>
        /// <param name="source">An object that contains properties that will be added as elements to the new collection.</param>
        /// <returns></returns>
        public static IDictionary<string, object> BuildHtmlAttributeDictionary(this object source)
        {
            return new RouteValueDictionary(source);
        }
    }
}