namespace RioValleyChili.Services.Interfaces.Returns.IntraWarehouseOrderService
{
    public interface IIntraWarehouseOrderSummaryReturn : IInventoryOrderSummaryReturn
    {
        decimal TrackingSheetNumber { get; }
        string OperatorName { get; }
    }
}