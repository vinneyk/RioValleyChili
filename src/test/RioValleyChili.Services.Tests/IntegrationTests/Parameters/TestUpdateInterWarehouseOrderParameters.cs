using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseOrderService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class TestUpdateInterWarehouseOrderParameters : IUpdateInterWarehouseOrderParameters
    {
        public string UserToken { get; set; }
        public string SourceFacilityKey { get; set; }
        public string DestinationFacilityKey { get; set; }
        public string InventoryShipmentOrderKey { get; set; }
        public ISetOrderHeaderParameters HeaderParameters { get; set; }
        public ISetShipmentInformation SetShipmentInformation { get; set; }
        public IEnumerable<ISetInventoryPickOrderItemParameters> InventoryPickOrderItems { get; set; }
        public IEnumerable<ISetPickedInventoryItemCodesParameters> PickedInventoryItemCodes { get; set; }
    }
}