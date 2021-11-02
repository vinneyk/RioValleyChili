using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SalesQuoteItemReturn : ISalesQuoteItemReturn
    {
        public string SalesQuoteItemKey { get { return SalesQuoteItemKeyReturn.SalesQuoteItemKey; } }
        public int Quantity { get; internal set; }
        public string CustomerProductCode { get; internal set; }
        public double PriceBase { get; internal set; }
        public double PriceFreight { get; internal set; }
        public double PriceTreatment { get; internal set; }
        public double PriceWarehouse { get; internal set; }
        public double PriceRebate { get; internal set; }
        public string TreatmentKey { get { return InventoryTreatmentKeyReturn.InventoryTreatmentKey; } }
        public IProductReturn Product { get; internal set; }
        public IPackagingProductReturn Packaging { get; internal set; }

        internal SalesQuoteItemKeyReturn SalesQuoteItemKeyReturn { get; set; }
        internal InventoryTreatmentKeyReturn InventoryTreatmentKeyReturn { get; set; }
    }
}