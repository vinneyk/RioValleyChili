namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Production
{
    public class ProductionResultItemDto
    {
        public string PackagingKey { get; set; }
        public string LocationKey { get; set; }
        public string InventoryTreatmentKey { get; set; }
        public int Quantity { get; set; }
    }
}