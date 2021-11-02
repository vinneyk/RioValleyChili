using System.Collections.Generic;
using System.Linq;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Models
{
    public static class DataBindingAttributeDictionaryExtensions
    {
        public static IDictionary<string, object> ForCheckbox(this IDictionary<string,object> attributes)
        {
            var dataBindingAttributes = ParseDataBindingArgumentsFromHtmlAttributeCollection(attributes).ToList();
            if (!dataBindingAttributes.Any())
            {
                return attributes;
            }

            var dataBindingDictionary = dataBindingAttributes.ToDictionary(m => m.Key, m => m.Value);
            ReplaceDictionaryEntryByKey(dataBindingDictionary, "value", "checked");
            ReplaceDictionaryEntryByKey(dataBindingDictionary, "text", "checked");

            var reconstructedArgs = from arg in dataBindingDictionary
                                    select string.Format("{0}:{1}", arg.Key, arg.Value);

            attributes["data-bind"] = string.Join(",", reconstructedArgs);

            return attributes;
        }

        private static IEnumerable<KeyValuePair<string, string>> ParseDataBindingArgumentsFromHtmlAttributeCollection(this IDictionary<string, object> attributes)
        {
            const string dataBindAttributeKey = "data-bind";
            if (!attributes.ContainsKey(dataBindAttributeKey))
            {
                yield break;
            }

            var dataBindingAttributeValue = attributes[dataBindAttributeKey].ToString();
            var dataBindingArgs = dataBindingAttributeValue.Split(',');
            foreach (var arg in dataBindingArgs)
            {
                var argSplit = arg.Split(':');
                yield return new KeyValuePair<string, string>(argSplit[0].Trim(), argSplit[1].Trim());
            }
        }

        private static void ReplaceDictionaryEntryByKey(IDictionary<string, string> attributes, string oldKey, string newKey)
        {
            if (attributes.ContainsKey(oldKey))
            {
                attributes[newKey] = attributes[oldKey];
                attributes.Remove(oldKey);
            }
        }
    }
}