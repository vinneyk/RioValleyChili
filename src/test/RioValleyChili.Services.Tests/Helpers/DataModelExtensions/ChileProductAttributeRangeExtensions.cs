using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ChileProductAttributeRangeExtensions
    {
        internal static ChileProductAttributeRange ConstrainByKeys(this ChileProductAttributeRange range, IChileProductKey chileProductKey = null, IAttributeNameKey attributeNameKey = null)
        {
            if(range == null) { throw new ArgumentNullException("range"); }

            if(chileProductKey != null)
            {
                range.ChileProduct = null;
                range.ChileProductId = chileProductKey.ChileProductKey_ProductId;
            }

            if(attributeNameKey != null)
            {
                range.AttributeName = null;
                range.AttributeShortName = attributeNameKey.AttributeNameKey_ShortName;
            }

            return range;
        }

        internal static ChileProductAttributeRange SetValues(this ChileProductAttributeRange range, double? min = null, double? max = null, string attributeShortName = null)
        {
            if(range == null) { throw new ArgumentNullException("range"); }

            if(attributeShortName != null)
            {
                range.AttributeShortName = attributeShortName;
            }
            
            if(min != null)
            {
                range.RangeMin = (double) min;
            }

            if(max != null)
            {
                range.RangeMax = (double) max;
            }

            return range;
        }

        internal static ChileProductAttributeRange SetValues(this ChileProductAttributeRange range, IAttributeNameKey attribute = null, double? min = null, double? max = null)
        {
            range.SetValues(min, max, attribute != null ? attribute.AttributeNameKey_ShortName : null);
            if(attribute != null)
            {
                range.AttributeName = null;
            }

            return range;
        }
    }
}