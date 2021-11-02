using System;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models.Helpers
{
    public static class IAttributeDefectExtensions
    {
        public static bool TooHigh(this IAttributeDefect defect)
        {
            return defect.Value > defect.RangeMax;
        }

        public static bool TooLow(this IAttributeDefect defect)
        {
            return defect.Value < defect.RangeMin;
        }

        /// <summary>
        /// The magnitude of the range, always positive.
        /// </summary>
        public static double AbsoluteRangeDelta(this IAttributeDefect defect)
        {
            return Math.Abs(defect.RangeMax - defect.RangeMin);
        }

        /// <summary>
        /// The magnitude of the difference between the value and the range limit broken, 0 if value is in range.
        /// </summary>
        public static double AbsoluteValueDelta(this IAttributeDefect defect)
        {
            if(defect.TooLow())
            {
                return defect.RangeMin - defect.Value;
            }

            if(defect.TooHigh())
            {
                return defect.Value - defect.RangeMax;
            }

            return 0.0;
        }

        /// <summary>
        /// The difference between the value and the range limit broken, normalized according to the magnitude of the range.
        /// If the range magnitude is zero, this will return null. Otherwise always positive.
        /// </summary>
        public static double? NormalizedValueDelta(this IAttributeDefect defect)
        {
            var rangeDelta = defect.AbsoluteRangeDelta();
            if(rangeDelta == 0.0)
            {
                return null;
            }

            return defect.AbsoluteValueDelta() / rangeDelta;
        }
    }
}