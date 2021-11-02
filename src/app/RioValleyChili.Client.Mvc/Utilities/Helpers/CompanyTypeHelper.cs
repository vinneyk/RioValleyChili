using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Client.Mvc.Core;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public static class CompanyTypeHelper
    {
        public static IEnumerable<CompanyType> KnownVendorTypes
        { 
            get
            {
                return new[]
                           {
                               CompanyType.Dehydrator, 
                               CompanyType.Freight, 
                               CompanyType.Supplier, 
                               CompanyType.TreatmentFacility
                           };
            }
        }

        public static IEnumerable<CompanyType> KnownCustomerTypes
        {
            get
            {
                return new[]
                           {
                               CompanyType.Broker,
                               CompanyType.Customer,
                           };
            }
        } 

        public static bool IsVendor(this ICompanySummaryReturn company)
        {
            return IsVendorExpression.Compile().Invoke(company);
        }

        public static Expression<Func<ICompanySummaryReturn, bool>> IsVendorExpression
        {
            get
            {
                return (company) => company.CompanyTypes.Any(KnownVendorTypes.Contains);
            }
        }

        public static CompanyType ToCompanyType(this VendorType vendorType)
        {
            switch (vendorType)
            {
                case VendorType.Dehydrator: return CompanyType.Dehydrator;
                case VendorType.Freight: return CompanyType.Freight;
                case VendorType.Supplier: return CompanyType.Supplier;
                case VendorType.TreatmentFacility: return CompanyType.TreatmentFacility;
                default: throw new NotSupportedException(string.Format("The conversion of VendorType \"{0}\" ({1}) is not supported.", vendorType, (int)vendorType));
            }
        }
    }
}