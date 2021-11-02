using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class WorkTypeProjectors
    {
        internal static Expression<Func<WorkType, WorkTypeKeyReturn>> SelectWorkTypeKey()
        {
            return w => new WorkTypeKeyReturn
                {
                    WorkTypeKey_WorkTypeId = w.Id
                };
        }

        internal static Expression<Func<WorkType, WorkTypeReturn>> Select()
        {
            var workTypeKey = SelectWorkTypeKey();

            return w => new WorkTypeReturn
                {
                    WorkTypeKeyReturn = workTypeKey.Invoke(w),
                    Description = w.Description
                };
        }
    }
}