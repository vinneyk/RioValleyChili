using System.Net.Http;
using System.Net.Http.Formatting;

namespace RioValleyChili.Client.Mvc.Extensions
{
    public static class ObjectExtensions
    {
        public static ObjectContent<T> ToJSONContent<T>(this T o)
        {
            return new ObjectContent<T>(o, new JsonMediaTypeFormatter());
        }
    }
}