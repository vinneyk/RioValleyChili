using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class LotAttributeExtensions
    {
        internal static LotAttribute SetValues(this LotAttribute lotAttribute, ILotKey lotKey = null, IAttributeNameKey attributeNameKey = null, double? value = null, bool? computed = null)
        {
            if(lotKey != null)
            {
                lotAttribute.Lot = null;
                lotAttribute.LotDateCreated = lotKey.LotKey_DateCreated;
                lotAttribute.LotDateSequence = lotKey.LotKey_DateSequence;
                lotAttribute.LotTypeId = lotKey.LotKey_LotTypeId;
            }

            if(attributeNameKey != null)
            {
                lotAttribute.AttributeName = null;
                lotAttribute.AttributeShortName = attributeNameKey.AttributeNameKey_ShortName;
            }

            if(value != null)
            {
                lotAttribute.AttributeValue = (double) value;
            }

            if(computed != null)
            {
                lotAttribute.Computed = computed.Value;
            }

            return lotAttribute;
        }

        internal static LotAttribute SetValues(this LotAttribute lotAttribute, IAttributeNameKey attributeNameKey = null, double? value = null, bool? computed = null)
        {
            return lotAttribute.SetValues(null, attributeNameKey, value, computed);
        }
    }
}