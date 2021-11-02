using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Interfaces.Returns
{
    public interface IPickOrderItemReturn
    {
        string OrderItemKey { get; }
        string ProductKey { get; }
        string ProductCode { get; }
        string ProductName { get; }
        string PackagingProductKey { get; }
        string PackagingName { get; }
        double PackagingWeight { get;}
        string TreatmentKey { get; }
        string TreatmentNameShort { get; }
        int Quantity { get; }
        double TotalWeight { get; }
        string CustomerLotCode { get; }
        string CustomerProductCode { get; }
        ICompanyHeaderReturn Customer { get; }
    }
}