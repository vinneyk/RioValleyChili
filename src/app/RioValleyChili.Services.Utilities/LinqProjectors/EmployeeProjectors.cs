using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class EmployeeProjectors
    {
        internal static Expression<Func<Employee, EmployeeKeyReturn>> SelectKey()
        {
            return e => new EmployeeKeyReturn
                {
                    EmployeeKey_Id = e.EmployeeId
                };
        }

        internal static Expression<Func<Employee, UserSummaryReturn>> SelectSummary()
        {
            var keySelect = SelectKey();

            return e => new UserSummaryReturn
                {
                    Name = e.UserName,
                    EmployeeKeyReturn = keySelect.Invoke(e)
                };
        }
    }
}