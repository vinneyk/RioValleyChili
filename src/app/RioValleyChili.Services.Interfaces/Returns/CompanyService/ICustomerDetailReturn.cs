namespace RioValleyChili.Services.Interfaces.Returns.CompanyService
{
    public interface ICustomerDetailReturn : ICompanyDetailReturn
    {
        ICompanySummaryReturn BrokerSummary { get; }
    }
}