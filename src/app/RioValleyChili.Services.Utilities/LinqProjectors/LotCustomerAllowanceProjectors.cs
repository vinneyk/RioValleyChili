using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class LotCustomerAllowanceProjectors
    {
        internal static Expression<Func<LotCustomerAllowance, LotCustomerAllowanceReturn>> Select()
        {
            var customerKey = CustomerProjectors.SelectKey();

            return a => new LotCustomerAllowanceReturn
                {
                    CustomerKeyReturn = customerKey.Invoke(a.Customer),
                    CustomerName = a.Customer.Company.Name
                };
        }

        internal static Expression<Func<LotCustomerAllowance, CustomerKeyReturn>> SelectCustomerKey()
        {
            return a => new CustomerKeyReturn
                {
                    CustomerKey_Id = a.CustomerId
                };
        }
    }
}