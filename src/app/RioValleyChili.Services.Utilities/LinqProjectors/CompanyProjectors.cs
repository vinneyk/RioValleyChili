// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class CompanyProjectors
    {
        internal static Expression<Func<Company, CompanyKeyReturn>> SelectKey()
        {
            return c => new CompanyKeyReturn
                {
                    CompanyKey_Id = c.Id
                };
        }

        internal static Expression<Func<Company, CompanyHeaderReturn>> SelectHeader()
        {
            var key = SelectKey();

            return c => new CompanyHeaderReturn
                {
                    CompanyKeyReturn = key.Invoke(c),
                    Name = c.Name
                };
        }

        internal static Expression<Func<Company, CompanySummaryReturn>> SelectSummary()
        {
            return SplitSelectSummary().Merge();
        }

        internal static IEnumerable<Expression<Func<Company, CompanySummaryReturn>>> SplitSelectSummary()
        {
            return new Projectors<Company, CompanySummaryReturn>
                {
                    SelectHeader().Merge(c => new CompanySummaryReturn
                        {
                            Active = c.Active
                        }),
                    c => new CompanySummaryReturn
                        {
                            CompanyTypes = c.CompanyTypes.Select(t => (CompanyType)t.CompanyType),
                        }
                };
        }

        internal static IEnumerable<Expression<Func<Company, CompanyDetailReturn>>> SplitSelectDetail()
        {
            var customerProperties = SelectCustomerProperties();

            return new Projectors<Company, CompanyDetailReturn>
                {
                    SplitSelectSummary().Select(s => s.Merge(c => new CompanyDetailReturn { })),
                    c => new CompanyDetailReturn
                        {
                            Customer = new[] { c.Customer }.Where(u => u != null && u.Company.CompanyTypes.Any(t => t.CompanyType == (int)CompanyType.Customer)).Select(u => customerProperties.Invoke(u)).FirstOrDefault()
                        }
                };
        }

        #region Private Parts

        private static Expression<Func<Customer, CustomerProperties>> SelectCustomerProperties()
        {
            var header = SelectHeader();
            var note = CustomerNoteProjectors.SelectCustomerCompanyNote();

            return c => new CustomerProperties
                {
                    Broker = header.Invoke(c.Broker),
                    CustomerNotes = c.Notes.Select(n => note.Invoke(n))
                };
        }

        #endregion
    }
}

// ReSharper restore ConvertClosureToMethodGroup