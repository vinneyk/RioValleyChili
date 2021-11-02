using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LocationReturn : ILocationReturn
    {
        public string LocationKey { get { return LocationKeyReturn.LocationKey; } }
        public string Description { get; internal set; }
        /// <summary>
        /// Note that this refers to the Location's Facility Active state, not the Active state of the Location itself (for that, expect the Status property to be "InActive" or "Available") - RI 2016-12-5
        /// </summary>
        public bool Active { get; internal set; }
        public LocationStatus? Status { get; internal set; }
        public string FacilityKey { get { return FacilityKeyReturn.FacilityKey; } }
        public string FacilityName { get; internal set; }

        internal LocationKeyReturn LocationKeyReturn { get; set; }
        internal FacilityKeyReturn FacilityKeyReturn { get; set; }
    }
}