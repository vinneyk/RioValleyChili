using System;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class CustomerProductAttributeRangeProjectors
    {
        internal static Expression<Func<CustomerProductAttributeRange, CustomerChileProductAttributeRangeReturn>> Select()
        {
            var key = SelectKey();

            return Projector<CustomerProductAttributeRange>.To(r => new CustomerChileProductAttributeRangeReturn
                {
                    KeyReturn = key.Invoke(r),
                    AttributeShortName = r.AttributeShortName,
                    RangeMin = r.RangeMin,
                    RangeMax = r.RangeMax,
                    Active = r.Active
                });
        }

        private static Expression<Func<CustomerProductAttributeRange, CustomerChileProductAttributeRangeKeyReturn>> SelectKey()
        {
            return r => new CustomerChileProductAttributeRangeKeyReturn
                {
                    CustomerKey_Id = r.CustomerId,
                    ChileProductKey_ProductId = r.ChileProductId,
                    AttributeNameKey_ShortName = r.AttributeShortName
                };
        }
    }
}