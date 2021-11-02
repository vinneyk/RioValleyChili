using System;
using System.Linq;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class StringHelper
    {
        public static int GetYearFromString(string year)
        {
            if (year.Count() < 2 || year.Count() > 4)
            {
                throw new ArgumentException("year is not in expected format.");
            }

            switch (year.Count())
            {
                case 2:
                    year = "20" + year;
                    break;
                case 3:
                    year = "2" + year;
                    break;
            }

            return int.Parse(year);
        }

        public static string TrimTruncate(this string @string, int maxLength)
        {
            return @string == null ? null : new string(@string.Trim().Take(maxLength).ToArray());
        }
    }
}
