using System;
using System.Globalization;
using System.Text.RegularExpressions;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class LotNumberParser
    {
        private const string INV_TYPE = "type";
        private const string YEAR = "year";
        private const string JULDATE = "julian";
        private const string SEQUENCE = "seq";
        private const int YearCenturyCutoff = 1960;

        private static readonly Regex LotNumberRegex = new Regex(string.Format(@"(?<{0}>\d+)(?<{1}>\d{{2}})(?<{2}>\d{{3}})(?<{3}>\d{{2}})",
                INV_TYPE,
                YEAR,
                JULDATE,
                SEQUENCE), RegexOptions.Compiled);

        public static int BuildLotNumber(ILotKey lotKey)
        {
            var lotString = string.Format("{0}{1:00}{2:000}{3:00}", lotKey.LotKey_LotTypeId, lotKey.LotKey_DateCreated.Year % 100, lotKey.LotKey_DateCreated.DayOfYear, lotKey.LotKey_DateSequence);
            return int.Parse(lotString);
        }

        public static bool ParseLotNumber(int lotNumber, out LotKey lotKey)
        {
            lotKey = null;

            var lotNumberString = lotNumber.ToString(CultureInfo.InvariantCulture);
            var match = LotNumberRegex.Match(lotNumberString);
            if(!match.Success)
            {
                return false;
            }

            var typeValue = match.Groups[INV_TYPE].Value;
            var yearValue = match.Groups[YEAR].Value;
            var julianValue = match.Groups[JULDATE].Value;
            var seqValue = match.Groups[SEQUENCE].Value;

            var year = int.Parse(yearValue);
            var century = "20";
            if(year > YearCenturyCutoff % 100)
            {
                century = "19";
            }

            yearValue = string.Format("{0}{1:00}", century, year);

            var lotDate = new DateTime(int.Parse(yearValue), 1, 1);
            lotDate = lotDate.AddDays(Math.Max(int.Parse(julianValue) - 1, 0));

            if(lotDate.Year < YearCenturyCutoff)
            {
                return false;
            }

            lotKey = new LotKey(new LotKeyParameters
                {
                    LotKey_DateCreated = lotDate.Date,
                    LotKey_DateSequence = int.Parse(seqValue),
                    LotKey_LotTypeId = int.Parse(typeValue)
                });

            return true;
        }

        public static LotKey ParseLotNumber(int lotNumber)
        {
            LotKey lotKey;
            return ParseLotNumber(lotNumber, out lotKey) ? lotKey : null;
        }

        private struct LotKeyParameters : ILotKey
        {
            public DateTime LotKey_DateCreated { get; set; }
            public int LotKey_DateSequence { get; set; }
            public int LotKey_LotTypeId { get; set; }
        }
    }
}
