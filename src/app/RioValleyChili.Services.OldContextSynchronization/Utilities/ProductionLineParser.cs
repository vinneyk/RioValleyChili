using System;
using System.Text.RegularExpressions;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.OldContextSynchronization.Utilities
{
    public static class ProductionLineParser
    {
        private static Regex _lineRegex = new Regex(@"Line.*?(\d+)", RegexOptions.Compiled);

        public static int GetProductionLineNumber(Location productionLocation)
        {
            var match = _lineRegex.Match(productionLocation.Description);
            if(!match.Success)
            {
                throw new Exception(string.Format("Could not parse line number from ProductionLocation.Description[{0}]", productionLocation.Description));
            }

            return int.Parse(match.Groups[1].Value);
        }
    }
}