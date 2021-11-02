using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales
{
    public class SalesOrderPickOrderItemResponse : InventoryPickOrderItemResponse
    {
        public string ContractItemKey { get; set; }
        public string ContractKey { get; set; }
                
        public double PriceBase { get; set; }
        public double PriceFreight { get; set; }
        public double PriceTreatment { get; set; }
        public double PriceWarehouse { get; set; }
        public double PriceRebate { get; set; }
    }
}