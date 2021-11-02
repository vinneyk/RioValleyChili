using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;

namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public interface ISetOrderParameters : IUserIdentifiable
    {
        string SourceFacilityKey { get; }
        string DestinationFacilityKey { get; }

        ISetOrderHeaderParameters HeaderParameters { get; }
        ISetShipmentInformation SetShipmentInformation { get; }

        IEnumerable<ISetInventoryPickOrderItemParameters> InventoryPickOrderItems { get; }
    }
}