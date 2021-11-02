using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ICustomerContractOrderItemReturn
    {
        string OrderItemKey { get; }

        string ContractItemKey { get; }

        IProductReturn Product { get; }

        IPackagingProductReturn Packaging { get; }

        IInventoryTreatmentReturn Treatment { get; }

        int TotalQuantityPicked { get; }

        int TotalWeightPicked { get; }

        double TotalPrice { get; }
    }
}