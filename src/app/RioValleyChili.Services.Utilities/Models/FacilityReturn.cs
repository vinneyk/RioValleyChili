using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class FacilityReturn : IFacilityDetailReturn
    {
        public string FacilityKey { get { return FacilityKeyReturn.FacilityKey; } }
        public string FacilityName { get; internal set; }
        public bool Active { get; internal set; }
        public IShippingLabelReturn ShippingLabel { get; internal set; }

        public FacilityType FacilityType { get; internal set; }
        public IEnumerable<ILocationReturn> Locations { get; internal set; }

        internal FacilityKeyReturn FacilityKeyReturn { get; set; }
    }
}