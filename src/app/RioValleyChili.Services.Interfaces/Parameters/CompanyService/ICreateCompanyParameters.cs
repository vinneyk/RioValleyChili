namespace RioValleyChili.Services.Interfaces.Parameters.CompanyService
{
    public interface ICreateCompanyParameters : ISetCompanyParameters
    {
        string CompanyName { get; }
        
    }
}