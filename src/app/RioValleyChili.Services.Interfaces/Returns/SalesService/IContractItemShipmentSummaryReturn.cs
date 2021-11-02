using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface IContractItemShipmentSummaryReturn
    {
        string ContractItemKey { get; }
        string CustomerProductCode { get; }

        double BasePrice { get; }
        double TotalValue { get; }
        int TotalWeight { get; }
        int TotalWeightShipped { get; }
        int TotalWeightPending { get; }
        int TotalWeightRemaining { get; }

        IProductReturn ChileProduct { get; }
        IPackagingProductReturn PackagingProduct { get; }
        IInventoryTreatmentReturn Treatment { get; }
    }
}