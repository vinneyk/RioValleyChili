namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Warehouse
{
    public class InterWarehouseOrderDetails :
        InventoryShipmentOrderBase
            <InterWarehouseOrderDetails, InventoryPickOrderDetail, Inventory.InventoryPickOrderItemResponse>
    {
    }
}