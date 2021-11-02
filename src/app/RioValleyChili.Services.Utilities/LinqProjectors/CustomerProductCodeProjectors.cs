using System;
using System.Linq.Expressions;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class CustomerProductCodeProjectors
    {
        internal static Expression<Func<CustomerProductCode, CustomerProductCodeReturn>> Select()
        {
            return c => new CustomerProductCodeReturn
                {
                    Value = c.Code
                };
        }
    }
}