namespace RioValleyChili.Services.Interfaces.Parameters.WarehouseService
{
    public interface IUpdateLocationParameters : ISaveLocationParameters
    {
        string LocationKey { get; }
    }
}