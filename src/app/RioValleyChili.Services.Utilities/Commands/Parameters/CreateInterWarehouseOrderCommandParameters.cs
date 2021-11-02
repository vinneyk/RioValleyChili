using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class CreateInterWarehouseOrderCommandParameters : InterWarehouseOrderCommandParameters
    {
        public IShipmentDetailReturn Shipment { get; set; }
    }
}