using System.Web.Mvc;
using System.Web.Routing;

namespace RioValleyChili.Client.Mvc.App_Start
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            
            routes.MapRoute(
                "SiteStatus",
                "site/status",
                MVC.Home.SiteStatus(),
                new[] { "RioValleyChili.Client.Mvc.Controllers" });

            routes.MapRoute(
                "SiteStartup",
                "site/startup",
                MVC.Home.Startup(),
                new[] { "RioValleyChili.Client.Mvc.Controllers" });

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "RioValleyChili.Client.Mvc.Controllers" }
                );
        }
    }
}