namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class SetPickedInventoryItemCodesRequestParameter
    {
        public string PickedInventoryItemKey { get; set; }
        public string CustomerProductCode { get; set; }
        public string CustomerLotCode { get; set; }
    }
}