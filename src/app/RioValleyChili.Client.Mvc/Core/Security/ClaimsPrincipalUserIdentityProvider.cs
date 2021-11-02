using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using Thinktecture.IdentityModel.Tokens.Http;

namespace RioValleyChili.Client.Mvc.Core.Security
{
    [ExtractIntoSolutionheadLibrary]
    public class ClaimsPrincipalUserIdentityProvider : IUserIdentityProvider
    {
        private readonly IPrincipal _principal;

        public ClaimsPrincipalUserIdentityProvider()
            : this(GetContextBaseFromCurrent()) { }

        public ClaimsPrincipalUserIdentityProvider(HttpContextBase context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            _principal = context.User;
        }

        public TPackage SetUserIdentity<TPackage>(TPackage package)
            where TPackage : IUserIdentifiable
        {
            if (!_principal.Identity.IsAuthenticated) { throw new AuthenticationException("The user is unauthenticated."); }
            
            var claimsPrincipal = _principal as ClaimsPrincipal;
            if (claimsPrincipal == null) { throw new ApplicationException("HttpContext.Current.User could not be converted to a ClaimsPrincipal."); }

            var userTokenClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserToken);
            if (userTokenClaim == null) { throw new ApplicationException("The current user's ClaimsPrincipal does not contain a \"UserToken\" claim."); }

            package.UserToken = userTokenClaim.Value;
            return package;
        }

        private static HttpContextBase GetContextBaseFromCurrent()
        {
            return new HttpContextWrapper(HttpContext.Current);
        }
    }
}