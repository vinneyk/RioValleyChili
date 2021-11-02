using System.Collections.Generic;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class ChileMaterialsItemReceivedKeys
    {
        public int Lot { get; private set; }
        public string Tote { get; private set; }
        public int PkgID { get; private set; }
        public int LocID { get; private set; }
        public int TrtmtID { get; private set; }
        public string DehyLocale { get; private set; }
        public string Variety { get; private set; }

        public ChileMaterialsItemReceivedKeys(tblIncoming tblIncoming)
        {
            Lot = tblIncoming.Lot;
            Tote = (tblIncoming.Tote ?? "").Trim();
            PkgID = tblIncoming.PkgID;
            LocID = tblIncoming.LocID;
            TrtmtID = tblIncoming.TrtmtID;
            DehyLocale = (tblIncoming.DehyLocale ?? "").Trim().ToUpper();
            Variety = ((tblIncoming.tblVariety == null ? null : tblIncoming.tblVariety.Variety) ?? "").Trim();
        }

        public static IEqualityComparer<ChileMaterialsItemReceivedKeys> ChileMaterialsItemReceivedKeysComparer { get { return ChileMaterialsItemReceivedKeysComparerInstance; } }
        private static readonly IEqualityComparer<ChileMaterialsItemReceivedKeys> ChileMaterialsItemReceivedKeysComparerInstance = new ChileMaterialsItemReceivedKeysEqualityComparer();
    }

    public sealed class ChileMaterialsItemReceivedKeysEqualityComparer : IEqualityComparer<ChileMaterialsItemReceivedKeys>
    {
        public bool Equals(ChileMaterialsItemReceivedKeys x, ChileMaterialsItemReceivedKeys y)
        {
            if(x == y) { return true; }
            return x.Lot == y.Lot &&
                   x.Tote == y.Tote &&
                   x.PkgID == y.PkgID &&
                   x.Variety == y.Variety &&
                   x.LocID == y.LocID &&
                   x.TrtmtID == y.TrtmtID &&
                   x.DehyLocale == y.DehyLocale;
        }

        public int GetHashCode(ChileMaterialsItemReceivedKeys obj)
        {
            unchecked
            {
                var hashCode = obj.Lot;
                hashCode = (hashCode * 397) ^ (obj.Tote != null ? obj.Tote.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.PkgID;
                hashCode = (hashCode * 397) ^ obj.Variety.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.LocID;
                hashCode = (hashCode * 397) ^ obj.TrtmtID;
                hashCode = (hashCode * 397) ^ (obj.DehyLocale != null ? obj.DehyLocale.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}