namespace RioValleyChili.Services.Interfaces.Parameters
{
    public interface ICancelTransactionParameters
    {
        string UserToken { get; }

        string TransactionKey { get; }
    }
}