using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Core
{
    public enum VendorType : short
    {
        Supplier = CompanyType.Supplier,
        Dehydrator = CompanyType.Dehydrator,
        Freight = CompanyType.Freight,
        TreatmentFacility = CompanyType.TreatmentFacility,
    }
}