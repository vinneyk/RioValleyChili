using System;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Services.Utilities.Helpers
{
    internal static class ToteKeyHelper
    {
        internal static string ToToteKey(this string @string)
        {
            if(@string == null) { return ""; }

            var toteKey = @string.Trim();
            toteKey = toteKey.Substring(0, Math.Min(toteKey.Length, Constants.StringLengths.ToteKeyLength));

            return toteKey;
        }
    }
}