using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ChileMaterialsReceivedExtensions
    {
        internal static ChileMaterialsReceived SetSupplier(this ChileMaterialsReceived received, ICompanyKey companyKey)
        {
            if(companyKey == null) { throw new ArgumentNullException("companyKey"); }

            received.Supplier = null;
            received.SupplierId = companyKey.CompanyKey_Id;
            return received;
        }
    }
}