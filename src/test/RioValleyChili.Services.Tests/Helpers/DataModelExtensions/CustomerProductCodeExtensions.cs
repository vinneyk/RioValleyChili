using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class CustomerProductCodeExtensions
    {
        internal static CustomerProductCode ConstrainByKeys(this CustomerProductCode productCode, ICustomerKey customerKey, IChileProductKey chileProductKey = null)
        {
            if(productCode == null) { throw new ArgumentNullException("productCode"); }

            if(customerKey != null)
            {
                productCode.Customer = null;
                productCode.CustomerId = customerKey.CustomerKey_Id;
            }

            if(chileProductKey != null)
            {
                productCode.ChileProduct = null;
                productCode.ChileProductId = chileProductKey.ChileProductKey_ProductId;
            }

            return productCode;
        }
    }
}