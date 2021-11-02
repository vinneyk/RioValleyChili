using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class CustomerProductAttributeRangeExtensions
    {
        internal static CustomerProductAttributeRange ConstrainByKeys(this CustomerProductAttributeRange range, ICustomerKey customerKey, IChileProductKey chileProductKey = null)
        {
            if(range == null) { throw new ArgumentNullException("range"); }

            if(customerKey != null)
            {
                range.Customer = null;
                range.CustomerId = customerKey.CustomerKey_Id;
            }

            if(chileProductKey != null)
            {
                range.ChileProduct = null;
                range.ChileProductId = chileProductKey.ChileProductKey_ProductId;
            }

            return range;
        }

        internal static CustomerProductAttributeRange SetValues(this CustomerProductAttributeRange range, ICustomerKey customer = null, IAttributeNameKey attribute = null, double? min = null, double? max = null,
            IChileProductKey chileProduct = null, bool active = true)
        {
            if(range == null) {  throw new ArgumentNullException("range"); }

            if(customer != null)
            {
                range.Customer = null;
                range.CustomerId = customer.CustomerKey_Id;
            }

            if(attribute != null)
            {
                range.AttributeName = null;
                range.AttributeShortName = attribute.AttributeNameKey_ShortName;
            }

            if(min != null)
            {
                range.RangeMin = min.Value;
            }

            if(max != null)
            {
                range.RangeMax = max.Value;
            }

            if(chileProduct != null)
            {
                range.ChileProduct = null;
                range.ChileProductId = chileProduct.ChileProductKey_ProductId;
            }

            range.Active = active;

            return range;
        }
    }
}