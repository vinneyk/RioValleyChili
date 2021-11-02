using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class MillAndWetdownResultItem 
    {
        public string MillAndWetdownResultItemKey { get; set; }

        public PackagingProductResponse PackagingProduct { get; set; }

        public FacilityLocationResponse Location { get; set; }

        public int QuantityProduced { get; set; }

        public int TotalWeightProduced { get; set; }
    }
}