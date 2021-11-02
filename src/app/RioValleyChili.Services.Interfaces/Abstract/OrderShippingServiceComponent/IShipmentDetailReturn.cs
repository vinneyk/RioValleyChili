using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent
{
    //todo: introduce IShipmentDetailParameters with IUserIdentifiable and without InventoryOrderEnum property. vk 11/22/13
    public interface IShipmentDetailReturn : IShipmentInformationReturn
    {
        InventoryOrderEnum InventoryOrderEnum { get; set; }
    }
}