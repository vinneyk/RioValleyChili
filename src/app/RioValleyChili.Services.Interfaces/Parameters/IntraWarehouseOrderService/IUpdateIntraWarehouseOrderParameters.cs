namespace RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService
{
    public interface IUpdateIntraWarehouseOrderParameters : IIntraWarehouseOrderParameters
    {
        string IntraWarehouseOrderKey { get; }
    }
}