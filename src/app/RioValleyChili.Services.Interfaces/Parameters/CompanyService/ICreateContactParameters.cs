using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.CompanyService
{
    public interface ICreateContactParameters : IContactParameters, IUserIdentifiable
    {
        string CompanyKey { get; }
    }
}