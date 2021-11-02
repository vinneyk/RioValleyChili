using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Extensions.Keys
{
    public static class ChileProductKeyExtensions
    {
        #region Extension.

        public static string BuildKey(this ChileProduct chileProduct)
        {
            return new ChileProductKey(chileProduct).ToString();
        }

        public static string BuildChileProductKey(this PackSchedule packSchedule)
        {
            if (packSchedule == null) { throw new ArgumentNullException("packSchedule"); }
            return new ChileProductKey(packSchedule).ToString();
        }

        public static ChileProductKey GetChileProductKey(this PackSchedule packSchedule)
        {
            if(packSchedule == null) { throw new ArgumentNullException("packSchedule"); }
            return new ChileProductKey(packSchedule);
        }

        #endregion
    }
}