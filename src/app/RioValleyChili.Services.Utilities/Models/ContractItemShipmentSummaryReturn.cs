using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ContractItemShipmentSummaryReturn : IContractItemShipmentSummaryReturn
    {
        public string ContractItemKey { get { return ContractItemKeyReturn.ContractItemKey; } }
        public string CustomerProductCode { get; internal set; }
        public double BasePrice { get; internal set; }
        public double TotalValue { get; internal set; }
        public int TotalWeight { get; internal set; }
        public int TotalWeightShipped { get; internal set; }
        public int TotalWeightPending { get; internal set; }
        public int TotalWeightRemaining { get; internal set; }
        public IProductReturn ChileProduct { get; internal set; }
        public IPackagingProductReturn PackagingProduct { get; internal set; }
        public IInventoryTreatmentReturn Treatment { get; internal set; }

        internal ContractItemKeyReturn ContractItemKeyReturn { get; set; }
    }
}