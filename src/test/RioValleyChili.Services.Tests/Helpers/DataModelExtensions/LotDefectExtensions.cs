using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class LotDefectExtensions
    {
        internal static LotDefect SetValues(this LotDefect lotDefect, ILotKey lotKey = null, DefectTypeEnum? defectType = null)
        {
            if(lotKey != null)
            {
                lotDefect.Lot = null;
                lotDefect.LotDateCreated = lotKey.LotKey_DateCreated;
                lotDefect.LotDateSequence = lotKey.LotKey_DateSequence;
                lotDefect.LotTypeId = lotKey.LotKey_LotTypeId;
            }

            if(defectType != null)
            {
                lotDefect.DefectType = (DefectTypeEnum) defectType;
            }

            return lotDefect;
        }
    }
}