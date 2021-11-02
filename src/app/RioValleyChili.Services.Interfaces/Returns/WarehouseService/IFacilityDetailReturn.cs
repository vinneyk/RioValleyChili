using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.WarehouseService
{
    public interface IFacilityDetailReturn : IFacilitySummaryReturn
    {
        FacilityType FacilityType { get; }
        IShippingLabelReturn ShippingLabel { get; }
        IEnumerable<ILocationReturn> Locations { get; }
    }
}