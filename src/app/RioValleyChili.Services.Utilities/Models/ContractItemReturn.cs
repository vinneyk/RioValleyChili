using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ContractItemReturn : IContractItemReturn
    {
        public string ContractItemKey { get { return ContractItemKeyReturn.ContractItemKey; } }

        public bool UseCustomerSpec { get; internal set; }
        public string CustomerProductCode { get; internal set; }
        public int Quantity { get; internal set; }
        public double PriceBase { get; internal set; }
        public double PriceFreight { get; internal set; }
        public double PriceTreatment { get; internal set; }
        public double PriceWarehouse { get; internal set; }
        public double PriceRebate { get; internal set; }

        public IChileProductReturn ChileProduct { get; internal set; }
        public IPackagingProductReturn PackagingProduct { get; internal set; }
        public IInventoryTreatmentReturn Treatment { get; internal set; }

        internal ContractItemKeyReturn ContractItemKeyReturn { get; set; }
    }
}