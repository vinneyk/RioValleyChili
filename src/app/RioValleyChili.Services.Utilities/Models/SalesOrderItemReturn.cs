using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SalesOrderItemReturn : InventoryPickOrderItemReturn, ISalesOrderItemReturn
    {
        public string ContractItemKey { get { return ContractItemKeyReturn.ContractItemKey; } }
        public string ContractKey { get { return ContractItemKeyReturn.ContractKey; } }
        
        public double PriceBase { get; internal set; }
        public double PriceFreight { get; internal set; }
        public double PriceTreatment { get; internal set; }
        public double PriceWarehouse { get; internal set; }
        public double PriceRebate { get; internal set; }

        #region Internal Parts

        internal SalesOrderItemKeyReturn SalesOrderItemKeyReturn { get; set; }
        internal NullableContractItemKeyReturn ContractItemKeyReturn { get; set; }

        #endregion
    }
}