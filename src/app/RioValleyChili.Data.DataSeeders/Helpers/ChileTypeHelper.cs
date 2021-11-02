using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Data.DataSeeders.Mothers.PocoMothers;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Helpers
{
    internal static class ChileTypeHelper
    {
        internal static ChileType GetChileType(int? prodGrpID, string productName)
        {
            switch(prodGrpID)
            {
                case 1:
                case 3685:
                    return ChileTypeMother.ChiliPepper;

                case 11:
                case 7:
                    return ChileTypeMother.Other;

                case 2: return ChileTypeMother.ChiliPowder;
                case 3: return ChileTypeMother.Paprika;
                case 4: return ChileTypeMother.RedPepper;
                case 12: return ChileTypeMother.GroundRedPepper;

                case 8:
                case 9:
                    return GetChileTypeFromName(productName);
            }

            return null;
        }

        internal static ChileType GetChileTypeFromName(string productName)
        {
            productName = productName.ToMatchFormat();
            foreach(var type in _chileTypeAliases)
            {
                if(type.Value.Any(d => productName.Contains(d)))
                {
                    return type.Key;
                }
            }

            return null;
        }

        static ChileTypeHelper()
        {
            _chileTypeAliases = new Dictionary<ChileType, IEnumerable<string>>
                {
                    { ChileTypeMother.ChiliPepper, new[] { "C.P.", "C.Pep.", "Guajillo" } },
                    { ChileTypeMother.ChiliPowder, new[] { "Powder" } },
                    { ChileTypeMother.Paprika, new[] { "Pap." } },
                    { ChileTypeMother.RedPepper, new[] { "Crushed Pepper" } },
                    { ChileTypeMother.GroundRedPepper, new string[0] }
                }.ToDictionary(t => t.Key, t => t.Value.Concat(new [] { t.Key.Description }).Select(n => n.ToMatchFormat()).ToList());
        }

        internal static readonly Dictionary<ChileType, List<string>> _chileTypeAliases;

        internal static string ToMatchFormat(this string s)
        {
            return s == null ? null : new string(s.ToUpper().ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
    }
}
