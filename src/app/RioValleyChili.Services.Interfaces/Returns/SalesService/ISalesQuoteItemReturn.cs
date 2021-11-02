using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ISalesQuoteItemReturn
    {
        string SalesQuoteItemKey { get; }
        int Quantity { get; }
        string CustomerProductCode { get; }
        double PriceBase { get; }
        double PriceFreight { get; }
        double PriceTreatment { get; }
        double PriceWarehouse { get; }
        double PriceRebate { get; }
        string TreatmentKey { get; }

        IProductReturn Product { get; }
        IPackagingProductReturn Packaging { get; }
    }
}