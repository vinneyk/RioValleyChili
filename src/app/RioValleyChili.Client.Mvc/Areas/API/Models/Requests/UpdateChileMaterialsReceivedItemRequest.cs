namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class UpdateChileMaterialsReceivedItemRequest
    {
        public string ItemKey { get; set; }
        public string GrowerCode { get; set; }
        public string ToteKey { get; set; }
        public int Quantity { get; set; }
        public string PackagingProductKey { get; set; }
        public string Variety { get; set; }
        public string LocationKey { get; set; }
    }
}