using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.SampleRequests
{
    public class SampleRequestItemResponse
    {
        public string ItemKey { get; set; }
        public string CustomerProductName { get; set; }
        public string LotKey { get; set; }
        public string ProductKey { get; set; }
        public ProductTypeEnum? ProductType { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }

        public SampleRequestItemSpecResponse CustomerSpec { get; set; }
        public SampleRequestItemLabResultsResponse LabResults { get; set; }

        public ResourceLinkCollection Links { get; set; }
    }
}