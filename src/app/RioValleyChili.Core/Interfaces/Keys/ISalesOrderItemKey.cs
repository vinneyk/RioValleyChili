namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface ISalesOrderItemKey : ISalesOrderKey
    {
        int SalesOrderItemKey_ItemSequence { get; }
    }
}