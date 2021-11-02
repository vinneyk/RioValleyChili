using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerContractOrderItemReturn : ICustomerContractOrderItemReturn
    {
        public string OrderItemKey { get { return SalesOrderItemKeyReturn.SalesOrderItemKey; } }
        public string ContractItemKey { get { return ContractItemKeyReturn.ContractItemKey; } }

        public int TotalQuantityPicked { get; internal set; }
        public int TotalWeightPicked { get; internal set; }
        public double TotalPrice { get; internal set; }

        public IProductReturn Product { get; internal set; }
        public IPackagingProductReturn Packaging { get; internal set; }
        public IInventoryTreatmentReturn Treatment { get; internal set; }

        internal SalesOrderItemKeyReturn SalesOrderItemKeyReturn { get; set; }
        internal NullableContractItemKeyReturn ContractItemKeyReturn { get; set; }
    }
}