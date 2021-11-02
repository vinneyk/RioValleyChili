using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class CompanyPredicates
    {
        internal static Expression<Func<Company, bool>> ByCompanyType(CompanyType companyType, bool includeInactive)
        {
            var includeInactiveCustomer = includeInactive && companyType == CompanyType.Customer;
            return c => c.CompanyTypes.Any(t => t.CompanyType == (int)companyType) || (includeInactiveCustomer && new[] { c.Customer }.Any(u => u != null));
        }

        internal static Expression<Func<Company, bool>> ByIncludeInactive(bool includeInactive)
        {
            return c => c.Active || includeInactive;
        }

        internal static Expression<Func<Company, bool>> ByBrokerKey(ICompanyKey broker, bool includeInactive)
        {
            var predicate = broker.ToCompanyKey().FindByPredicate;
            return c => ( includeInactive || c.CompanyTypes.Any(t => t.CompanyType == (int)CompanyType.Customer) )
                && new[] { c.Customer }.Where(u => u != null).Any(u => predicate.Invoke(u.Broker));
        }
    }
}