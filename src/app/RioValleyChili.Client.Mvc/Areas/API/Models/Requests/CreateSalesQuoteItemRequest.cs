namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class CreateSalesQuoteItemRequest
    {
        public int Quantity { get; set; }
        public string CustomerProductCode { get; set; }
        public double PriceBase { get; set; }
        public double PriceFreight { get; set; }
        public double PriceTreatment { get; set; }
        public double PriceWarehouse { get; set; }
        public double PriceRebate { get; set; }

        public string ProductKey { get; set; }
        public string PackagingKey { get; set; }
        public string TreatmentKey { get; set; }
    }
}