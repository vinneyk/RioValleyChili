namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales
{
    public class SalesQuoteItemResponse
    {
        public string SalesQuoteItemKey { get; set; }
        public int Quantity { get; set; }
        public string CustomerProductCode { get; set; }
        public double PriceBase { get; set; }
        public double PriceFreight { get; set; }
        public double PriceTreatment { get; set; }
        public double PriceWarehouse { get; set; }
        public double PriceRebate { get; set; }
        public string TreatmentKey { get; set; }

        public InventoryProductResponse Product { get; set; }
        public PackagingProductResponse Packaging { get; set; }
    }
}