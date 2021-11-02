using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class LotAttributeDefectExtensions
    {
        internal static LotAttributeDefect SetValues(this LotAttributeDefect defect, ILotKey lotKey = null, IAttributeNameKey attributeNameKey = null, DefectTypeEnum? defectType = null, double? value = null, double? rangeMin = null, double? rangeMax = null)
        {
            if(lotKey != null)
            {
                if(defect.LotDefect != null)
                {
                    defect.LotDefect.SetValues(lotKey);
                }

                defect.LotDateCreated = lotKey.LotKey_DateCreated;
                defect.LotDateSequence = lotKey.LotKey_DateSequence;
                defect.LotTypeId = lotKey.LotKey_LotTypeId;
            }

            if(attributeNameKey != null)
            {
                defect.AttributeName = null;
                defect.AttributeShortName = attributeNameKey.AttributeNameKey_ShortName;
            }

            if(defectType != null)
            {
                defect.LotDefect.DefectType = (DefectTypeEnum) defectType;
            }

            if(value != null)
            {
                defect.OriginalAttributeValue = (double) value;
            }

            if(rangeMin != null)
            {
                defect.OriginalAttributeMinLimit = (double) rangeMin;
            }

            if(rangeMax != null)
            {
                defect.OriginalAttributeMaxLimit = (double) rangeMax;
            }

            return defect;
        }
    }
}