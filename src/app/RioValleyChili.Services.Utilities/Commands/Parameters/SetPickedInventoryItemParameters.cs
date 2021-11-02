using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class SetPickedInventoryItemParameters : IPickedInventoryItemParameters
    {
        public string OrderItemKey { get; set; }
        public string InventoryKey { get; set; }
        public int Quantity { get; set; }
        public string CustomerLotCode { get; set; }
        public string CustomerProductCode { get; set; }

        public SetPickedInventoryItemParameters(IPickedInventoryItemParameters parameters)
        {
            OrderItemKey = parameters.OrderItemKey;
            InventoryKey = parameters.InventoryKey;
            Quantity = parameters.Quantity;
            CustomerLotCode = parameters.CustomerLotCode;
            CustomerProductCode = parameters.CustomerProductCode;
        }
    }
}