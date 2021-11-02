using System.Web.Mvc;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Core.Filters
{
    public class KillSwitchEngagedFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (InterceptRequest(filterContext) && !IsSiteStatusAction(filterContext))
            {
                filterContext.Result = new RedirectToRouteResult(MVC.Home.SiteStatus().GetRouteValueDictionary());
            }

            base.OnActionExecuting(filterContext);
        }

        private static bool InterceptRequest(ActionExecutingContext filterContext)
        {
            if (!KillSwitch.IsEngaged) return false;
            return !BypassKillswitchCheck(filterContext);
        }
        private static bool BypassKillswitchCheck(ActionExecutingContext filterContext)
        {
            return filterContext.ActionDescriptor.GetCustomAttributes(typeof (BypassKillSwitchFilter), false).Length == 0;
        }

        private static bool IsSiteStatusAction(ActionExecutingContext filterContext)
        {
            return filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == MVC.Home.Name &&
                   filterContext.ActionDescriptor.ActionName == MVC.Home.ActionNames.SiteStatus;
        }
    }
}