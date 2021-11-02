using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class ChileProductAttributeRangeExtensions
    {
        public static bool ValueOutOfRange(this IAttributeRange attributeRange, double? value)
        {
            if(attributeRange == null || value == null)
            {
                return false;
            }

            return value < attributeRange.RangeMin || value > attributeRange.RangeMax;
        }
    }
}