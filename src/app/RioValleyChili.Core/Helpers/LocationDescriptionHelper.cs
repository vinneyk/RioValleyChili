using System;
using System.Globalization;
using System.Linq;
using RioValleyChili.Core.Interfaces;

namespace RioValleyChili.Core.Helpers
{
    public static class LocationDescriptionHelper
    {
        public const char Separator = '~';

        public static string FormatLocationDescription(string formattedString)
        {
            if(string.IsNullOrWhiteSpace(formattedString))
            {
                return "";
            }
            
            var split = formattedString.Split(Separator);
            int rowNum;
            if(split.Length == 2 && int.TryParse(split[1], out rowNum))
            {
                return ToDisplayString(split[0], rowNum);
            }
            return string.Join(" ", split);
        }
        
        public static string ParseToDisplayString(string formattedString)
        {
            string street;
            int row;
            return GetStreetRow(formattedString, out street, out row) ? ToDisplayString(street, row) : formattedString;
        }

        public static string ParseToProductionLine(string formattedString)
        {
            string street;
            int row;
            return GetStreetRow(formattedString, out street, out row) ? string.Format("{0} {1}", street, row) : formattedString;
        }

        public static string ToDisplayString(string street, int row)
        {
            return string.Format("{0}{1:00}", street, row);
        }

        public static string GetDescription(string street, int row)
        {
            street = (street ?? "").Replace(Separator.ToString(CultureInfo.InvariantCulture), "");
            return string.Format("{0}{1}{2}", street, Separator, row);
        }

        public static bool GetStreetRow(string description, out string street, out int row)
        {
            if(string.IsNullOrWhiteSpace(description))
            {
                street = null;
                row = -1;
                return false;
            }

            street = description;
            row = 0;

            var parts = description.Split(new [] { Separator }, StringSplitOptions.None).ToList();
            if(parts.Count > 1)
            {
                int r;
                var lastPart = parts[parts.Count - 1];
                if(int.TryParse(lastPart, out r))
                {
                    street = description.Substring(0, description.Length - (Separator + lastPart).Count());
                    row = r;
                }
            }

            return true;
        }

        public static IQueryable<ILocationDescription> FilterByStreet(this IQueryable<ILocationDescription> source, string streetFilterValue)
        {
            var filter = string.Format("{0}{1}", streetFilterValue, Separator);
            return source.Where(s => s.Description.Contains(filter))
                .Select(s => s);
        }
    }
}