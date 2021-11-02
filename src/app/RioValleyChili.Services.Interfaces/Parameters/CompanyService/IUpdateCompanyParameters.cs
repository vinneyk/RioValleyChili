namespace RioValleyChili.Services.Interfaces.Parameters.CompanyService
{
    public interface IUpdateCompanyParameters : ISetCompanyParameters
    {
        string CompanyKey { get; }
    }
}