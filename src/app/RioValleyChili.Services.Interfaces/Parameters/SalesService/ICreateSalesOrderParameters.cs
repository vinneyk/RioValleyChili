namespace RioValleyChili.Services.Interfaces.Parameters.SalesService
{
    public interface ICreateSalesOrderParameters : ISetSalesOrderParameters
    {
        string CustomerKey { get; }
        bool IsMiscellaneous { get; }
    }
}