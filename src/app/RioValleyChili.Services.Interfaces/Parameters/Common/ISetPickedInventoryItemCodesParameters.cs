namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public interface ISetPickedInventoryItemCodesParameters
    {
        string PickedInventoryItemKey { get; }
        string CustomerProductCode { get; }
        string CustomerLotCode { get; }
    }
}