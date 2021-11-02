using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public static class AttributeNamesByProductTypeConverter
    {
        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> ConvertToPrimitiveTypes(this IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<string>>> source)
        {
            return source.Select(ConvertToPrimitiveTypes);
        }
        public static KeyValuePair<string, IEnumerable<string>> ConvertToPrimitiveTypes(this KeyValuePair<ProductTypeEnum, IEnumerable<string>> source)
        {
            return new KeyValuePair<string, IEnumerable<string>>(source.Key.ToString(), source.Value);
        }
    }
}