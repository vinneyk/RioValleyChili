using System;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class DBFunctions
    {
        [EdmFunction("SqlServer", "POWER")]
        public static double? Power(double? arg, int? pow)
        {
            if(arg == null || pow == null)
            {
                return null;
            }

            return Math.Pow((double)arg, (double)pow);
        }

        [EdmFunction("Edm", "DiffDays")]
        public static int? DiffDays(DateTime? dateValue1, DateTime? dateValue2)
        {
            if(dateValue1 == null || dateValue2 == null)
            {
                return null;
            }

            return (dateValue2 - dateValue1).Value.Days;
        }

        [EdmFunction("Edm", "Replace")]
        public static string Replace(string str, string sourceValue, string newValue)
        {
            return str.Replace(sourceValue, newValue);
        }

        [EdmFunction("SqlServer", "LEN")]
        public static int Len(string str)
        {
            return str.Count();
        }
    }
}