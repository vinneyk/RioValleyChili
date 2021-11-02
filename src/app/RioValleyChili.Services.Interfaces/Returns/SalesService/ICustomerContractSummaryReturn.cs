namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ICustomerContractSummaryReturn : ICustomerContractBaseReturn
    {
        double AverageBasePrice { get; }
        double AverageTotalPrice { get; }
        double SumQuantity { get; }
        double SumWeight { get; }
        double SumValue { get; }
    }
}