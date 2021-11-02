// ReSharper disable ConvertClosureToMethodGroup

using System.Collections.Generic;
using EF_Projectors;
using EF_Projectors.Extensions;
using RioValleyChili.Services.Utilities.Models.KeyReturns;
using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class CustomerProjectors
    {
        internal static Expression<Func<Customer, CompanyHeaderReturn>> SelectCompanyHeader()
        {
            var header = CompanyProjectors.SelectHeader();
            return c => header.Invoke(c.Company);
        }

        internal static Expression<Func<Customer, CompanySummaryReturn>> SelectCompanySummary()
        {
            var summary = CompanyProjectors.SelectSummary();
            return c => summary.Invoke(c.Company);
        }
        
        internal static Expression<Func<Customer, CustomerKeyReturn>> SelectKey()
        {
            return c => new CustomerKeyReturn
                {
                    CustomerKey_Id = c.Id
                };
        }

        internal static Expression<Func<Customer, int, CustomerWithProductSpecReturn>> SelectProductSpec()
        {
            var specSelect = CustomerProductAttributeRangeProjectors.Select();
            var keySelect = SelectKey();

            return (c, p) => new CustomerWithProductSpecReturn
                {
                    CustomerName = c.Company.Name,
                    CustomerKeyReturn = keySelect.Invoke(c),
                    AttributeRanges = c.ProductSpecs.Where(s => s.ChileProductId == p).Select(s => specSelect.Invoke(s))
                };
        }

        internal static Expression<Func<Customer, CustomerNotesReturn>> SelectNotes()
        {
            var note = CustomerNoteProjectors.Select();

            return CompanyProjectors.SelectHeader().Merge(Projector<Customer>.To(c => new CustomerNotesReturn
                {
                    CustomerNotes = c.Notes.Select(n => note.Invoke(n))
                }), c => c.Company);
        }

        internal static Expression<Func<Customer, IEnumerable<CustomerChileProductAttributeRangesReturn>>> SelectProductSpecs(bool onlyActive = false)
        {
            var customerKey = SelectKey();
            var productKey = ProductProjectors.SelectChileProductSummary();
            var rangeSelect = CustomerProductAttributeRangeProjectors.Select();

            return Projector<Customer>.To(c => c.ProductSpecs
                .Where(r => r.Active || !onlyActive)
                .GroupBy(r => r.ChileProduct)
                .Select(g => new CustomerChileProductAttributeRangesReturn
                    {
                        CustomerKeyReturn = customerKey.Invoke(c),
                        ChileProduct = productKey.Invoke(g.Key),
                        AttributeRanges = g.Select(r => rangeSelect.Invoke(r))
                    }));
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup