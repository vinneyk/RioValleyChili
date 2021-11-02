namespace RioValleyChili.Services.Interfaces.Parameters.SalesService
{
    public interface IUpdateSalesOrderParameters : ISetSalesOrderParameters
    {
        string SalesOrderKey { get; }
        bool CreditMemo { get; }
    }
}