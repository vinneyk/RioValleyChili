using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Core.Filters
{
    public class KillSwitchEngagedApiFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (KillSwitch.IsEngaged && !BypassKillswitchCheck(actionContext))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("The request was refused because the website's Killswitch has been engaged. Please contact the administrator if you have questions.")
                };
            }
            base.OnActionExecuting(actionContext);
        }

        private static bool BypassKillswitchCheck(HttpActionContext filterContext)
        {
            return filterContext.ActionDescriptor.GetFilters().OfType<BypassKillSwitchFilter>().Any();
        }
    }
}