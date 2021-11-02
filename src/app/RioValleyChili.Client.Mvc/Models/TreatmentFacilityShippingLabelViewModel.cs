using System.Collections.Generic;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Client.Mvc.Models
{
    public class TreatmentFacilityShippingLabelViewModel
    {
        public string TreatmentFacilityKey { get; set; }

        public string Name { get; set; }

        public Address ShippingAddress { get; set; }

        public IDictionary<string, string> TreatmentsPerformed { get; set; }
    }
}