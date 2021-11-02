using System;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace RioValleyChili.Client.Mvc.Core.Security
{
    public class CustomClaimsTransformer : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (!incomingPrincipal.Identity.IsAuthenticated)
            {
                return base.Authenticate(resourceName, incomingPrincipal);
            }
            CreateSession(incomingPrincipal);
            return incomingPrincipal;
        }

        private void CreateSession(ClaimsPrincipal claimsPrincipal)
        {
            var sessionSecurityToken = new SessionSecurityToken(claimsPrincipal, TimeSpan.FromHours(10));
            FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sessionSecurityToken);
        }
    }
}