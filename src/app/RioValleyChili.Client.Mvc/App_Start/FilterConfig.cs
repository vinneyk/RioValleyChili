using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core.Filters;

namespace RioValleyChili.Client.Mvc.App_Start
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new KillSwitchEngagedFilter());
            filters.Add(new AuthorizeAttribute());
        }
    }
}