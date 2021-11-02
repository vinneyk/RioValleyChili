using RioValleyChili.Core.Models;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared
{
    public class FacilityResponse 
    {
        public string FacilityKey { get; set; }
        public string FacilityName { get; set; }
        public bool Active { get; set; }
        public ShippingLabel ShippingLabel { get; set; }
    }
}