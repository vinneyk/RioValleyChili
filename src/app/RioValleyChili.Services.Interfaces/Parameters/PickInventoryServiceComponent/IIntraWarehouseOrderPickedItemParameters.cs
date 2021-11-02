namespace RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent
{
    public interface IIntraWarehouseOrderPickedItemParameters : IPickedInventoryItemParameters
    {
        string DestinationLocationKey { get; set; }
    }
}