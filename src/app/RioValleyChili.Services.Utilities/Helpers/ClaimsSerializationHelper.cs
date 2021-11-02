using System.Collections.Generic;
using Newtonsoft.Json;

namespace RioValleyChili.Services.Utilities.Helpers
{
    public static class ClaimsSerializationHelper
    {
        public static IDictionary<string, string> Deserialize(string input)
        {
            return string.IsNullOrWhiteSpace(input) 
                ? new Dictionary<string, string>()
                : JsonConvert.DeserializeObject<IDictionary<string, string>>(input);
        }

        public static string Serialize(IEnumerable<KeyValuePair<string, string>> claims)
        {
            return JsonConvert.SerializeObject(claims);
        }
    }
}