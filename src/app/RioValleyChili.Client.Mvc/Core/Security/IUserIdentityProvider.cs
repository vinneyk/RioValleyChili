using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Client.Mvc.Core.Security
{
    public interface IUserIdentityProvider
    {
        TPackage SetUserIdentity<TPackage>(TPackage package) where TPackage : IUserIdentifiable;
    }
}