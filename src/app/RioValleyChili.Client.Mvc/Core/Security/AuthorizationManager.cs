using System;
using System.Collections;
using System.Linq;
using System.Security.Claims;

namespace RioValleyChili.Client.Mvc.Core.Security
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            return IsAuthorized(context);
        }

        public bool IsAuthorized(AuthorizationContext context)
        {
            if (context.Action.Any(a => a.Value.Equals(ClaimTypes.Prerelease, StringComparison.OrdinalIgnoreCase)))
            {
                return IsGodMode(context.Principal);
            }

            var action = context.Action.First();
            return context.Resource.All(c => AuthorizeClaims(c, action, context.Principal));
        }

        private static readonly Func<Claim, bool> IsGodModeDelegate = (c) => c.Type.Equals(ClaimTypes.Solutionhead); 

        private static bool IsGodMode(ClaimsPrincipal principal)
        {
            return principal.Claims.Any(IsGodModeDelegate);
        }
        private static bool IsSuperUser(ClaimsPrincipal principal)
        {
            return principal.Claims.Any(c => c.Type.Equals(ClaimTypes.SuperUser) || IsGodModeDelegate(c));
        }
        
        private static bool AuthorizeClaims(Claim resourceClaim, Claim actionClaim, ClaimsPrincipal principal)
        {
            return IsSuperUser(principal) || principal.Claims.Where(c => c.Type.Equals(resourceClaim.Value, StringComparison.OrdinalIgnoreCase))
                .Any(c => c.Value.Equals("full", StringComparison.OrdinalIgnoreCase)  
                    || c.Value.Equals(actionClaim.Value, StringComparison.OrdinalIgnoreCase));
        }
    }
}