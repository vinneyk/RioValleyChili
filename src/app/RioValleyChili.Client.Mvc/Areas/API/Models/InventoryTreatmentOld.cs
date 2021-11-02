using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    [Obsolete("Use RioValleyChili.Client.Areas.Api.Models.InventoryTreatment instead.")]
    public class InventoryTreatmentOld
    {
        public string TreatmentKey { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }
    }
}