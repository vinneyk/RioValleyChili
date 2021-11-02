using System.Web.Http.Routing;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore
{
    public static class ActionLinkExtensions
    {
        public static void AdaptAction(this IActionLinkAdapter adapter, UrlHelper urlHelper)
        {
            if (adapter.RouteValues != null)
            {
                adapter.ActionUrl = urlHelper.Route("", adapter.RouteValues);
            }
        }
    }
}