using System.Linq;
using System.Security.Claims;
using System.Web;

namespace RioValleyChili.Client.Mvc.Core.Security
{
    public class VaryByClaimsPrincipalUserOutputCache
    {

        public string GetVaryByUserString(HttpContext context)
        {
            return context.User.Identity.IsAuthenticated
                ? GetAuthenticatedUserCacheToken(context)
                : string.Empty;
        }

        private string GetAuthenticatedUserCacheToken(HttpContext context)
        {
            var claimsPrincipal = context.User as ClaimsPrincipal;
            if (claimsPrincipal == null) return context.User.Identity.Name;
            var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(Core.Security.ClaimTypes.SessionToken));
            return claim == null
                ? context.User.Identity.Name
                : claim.Value;
        }

    }
}