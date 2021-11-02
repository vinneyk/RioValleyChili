using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared
{
    public class FacilityDetailsResponse
    {
        public string FacilityKey { get; set; }
        public string FacilityName { get; set; }
        public bool Active { get; set; }
        public ShippingLabel ShippingLabel { get; set; }

        public FacilityType FacilityType { get; set; }
        public IEnumerable<FacilityLocationResponse> Locations { get; set; }
    }
}