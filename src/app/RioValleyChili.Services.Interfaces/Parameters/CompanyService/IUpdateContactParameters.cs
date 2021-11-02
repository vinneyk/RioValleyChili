using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.CompanyService
{
    public interface IUpdateContactParameters : IContactParameters, IUserIdentifiable
    {
        string ContactKey { get; }
    }
}