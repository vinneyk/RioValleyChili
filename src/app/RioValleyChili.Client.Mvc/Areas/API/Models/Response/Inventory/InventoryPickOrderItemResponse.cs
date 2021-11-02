namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory
{
    public class InventoryPickOrderItemResponse
    {
        public string OrderItemKey { get; set; }
        public string ProductKey { get; set; }
        public string ProductName { get; set; }
        public string PackagingProductKey { get; set; }
        public string PackagingName { get; set; }
        public double PackagingWeight { get; set; }
        public string TreatmentKey { get; set; }
        public string TreatmentNameShort { get; set; }
        public int Quantity { get; set; }
        public double TotalWeight { get; set; }
        public string CustomerLotCode { get; set; }
        public string CustomerProductCode { get; set; }
        public CompanyResponse Customer { get; set; }
    }
}