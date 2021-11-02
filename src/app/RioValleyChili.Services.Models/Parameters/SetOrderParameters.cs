using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetOrderParameters : ISetOrderParameters
    {
        public string UserToken { get; set; }
        public string SourceFacilityKey { get; set; }
        public string DestinationFacilityKey { get; set; }

        public SetOrderHeaderParameters HeaderParameters { get; set; }
        public SetInventoryShipmentInformationParameters SetShipmentInformation { get; set; }

        public IEnumerable<SetInventoryPickOrderItemParameters> InventoryPickOrderItems { get; set; }

        #region explicit interface implementations

        ISetShipmentInformation ISetOrderParameters.SetShipmentInformation { get { return SetShipmentInformation; } }
        ISetOrderHeaderParameters ISetOrderParameters.HeaderParameters { get { return HeaderParameters; } }
        IEnumerable<ISetInventoryPickOrderItemParameters> ISetOrderParameters.InventoryPickOrderItems { get { return InventoryPickOrderItems; } }

        #endregion
    }
}