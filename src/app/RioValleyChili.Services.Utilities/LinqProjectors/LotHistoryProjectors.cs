using System;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LotHistoryProjectors
    {
        internal static Expression<Func<LotHistory, LotSerializedHistoryReturn>> Select()
        {
            var employee = EmployeeProjectors.SelectSummary();

            return Projector<LotHistory>.To(h => new LotSerializedHistoryReturn
                {
                    Employee = employee.Invoke(h.Employee),
                    Timestamp = h.TimeStamp,
                    Serialized = h.Serialized
                });
        }
    }
}