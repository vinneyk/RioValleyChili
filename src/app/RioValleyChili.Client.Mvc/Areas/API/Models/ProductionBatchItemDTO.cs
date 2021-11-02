namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class ProductionBatchItemDTO
    {
        public string InventoryLotNumber { get; set; }

        public string ProductionBatchLotNumber { get; set; }

        public int Quantity { get; set; }
    }
}
