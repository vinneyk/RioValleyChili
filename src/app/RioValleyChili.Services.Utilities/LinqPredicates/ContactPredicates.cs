using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.LinqPredicates
{
    internal static class ContactPredicates
    {
        internal static Expression<Func<Contact, bool>> ByCompanyKey(IKey<Company> companyKey)
        {
            var companyPredicate = companyKey.FindByPredicate;

            return c => companyPredicate.Invoke(c.Company);
        }
    }
}