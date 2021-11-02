using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface IContractItemReturn
    {
        string ContractItemKey { get; }

        IChileProductReturn ChileProduct { get; }

        IPackagingProductReturn PackagingProduct { get; }

        IInventoryTreatmentReturn Treatment { get; }

        bool UseCustomerSpec { get;  }

        string CustomerProductCode { get; }

        int Quantity { get; }

        double PriceBase { get; }

        double PriceFreight { get; }

        double PriceTreatment { get; }

        double PriceWarehouse { get; }

        double PriceRebate { get; }
    }
}