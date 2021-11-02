using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.ChileMaterialsReceived
{
    public class ChileMaterialsReceivedItemResponse 
    {
        public string ItemKey { get; set; }
        public string Variety { get; set; }
        public string ToteKey { get; set; }
        public int Quantity { get; set; }
        public string GrowerCode { set; get; }
        public int TotalWeight { get; set; }

        public FacilityLocationResponse Location { get; set; }
        public PackagingProductResponse PackagingProduct { get; set; }
    }
}