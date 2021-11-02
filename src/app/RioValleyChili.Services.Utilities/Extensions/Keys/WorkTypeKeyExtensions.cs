using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Extensions.Keys
{
    public static class WorkTypeKeyExtensions
    {
        #region Extension.

        public static string BuildKey(this WorkType workType)
        {
            return new WorkTypeKey(workType).KeyValue;
        }

        public static WorkTypeKey GetWorkTypeKey(this PackSchedule packSchedule)
        {
            if(packSchedule == null) { throw new ArgumentNullException("packSchedule"); }
            return new WorkTypeKey(packSchedule);
        }

        #endregion
    }
}
