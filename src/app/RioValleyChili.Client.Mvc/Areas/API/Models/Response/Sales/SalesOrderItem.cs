namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales
{
    public class SalesOrderItem
    {
        public string ContractItemKey { get; set; }
        public string ProductKey { get; set; }
        public string PackagingKey { get; set; }
        public string TreatmentKey { get; set; }
        public int Quantity { get; set; }
        public string CustomerLotCode { get; set; }
        public string CustomerProductCode { get; set; }
        public double PriceBase { get; set; }
        public double PriceFreight { get; set; }
        public double PriceTreatment { get; set; }
        public double PriceWarehouse { get; set; }
        public double PriceRebate { get; set; }
    }
}