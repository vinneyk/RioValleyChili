using System;
using System.Linq;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public static class StringExtensions
    {
        public static bool TryTruncate(this string thisString, int maxLength, out string truncatedString)
        {
            truncatedString = null;

            if(maxLength < 0)
            {
                throw new ArgumentException("maxLength cannot be negative");
            }

            if(thisString == null)
            {
                return false;
            }

            if(thisString == "")
            {
                truncatedString = "";
                return false;
            }

            var truncated = maxLength < thisString.Length;
            truncatedString = new string(thisString.Take(maxLength).ToArray());
            return truncated;
        }

        public static string AnyTruncate(this string thisString, int maxLength, ref bool truncated)
        {
            string truncatedString;
            if(thisString.TryTruncate(maxLength, out truncatedString))
            {
                truncated = true;
            }

            return truncatedString;
        }
    }
}