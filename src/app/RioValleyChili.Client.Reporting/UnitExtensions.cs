using Telerik.Reporting.Drawing;

namespace RioValleyChili.Client.Reporting
{
    public static class UnitExtensions
    {
        public static double ToPixels(this Unit unit)
        {
            return ToPixels(unit.Value, unit.Type);
        }

        public static double ToPixels(this double value, UnitType type)
        {
            switch(type)
            {
                case UnitType.Pixel: return value;

                case UnitType.Point:
                    value *= Unit.DotsPerInch / 72.0;
                    return value;

                case UnitType.Pica:
                    value *= Unit.DotsPerInch / 96.0 * 16.0;
                    return value;

                case UnitType.Inch:
                    value *= Unit.DotsPerInch;
                    return value;

                case UnitType.Mm:
                    value *= Unit.DotsPerInch / 96.0 * 3.77952766418457;
                    return value;

                case UnitType.Cm:
                    value *= Unit.DotsPerInch / 96.0 * 37.7952766418457;
                    return value;

                default: return value;
            }
        }

        public static Unit ToPoint(this Unit unit)
        {
            return new Unit(unit.ToPixels() / (Unit.DotsPerInch / 72.0), UnitType.Point);
        }
    }
}