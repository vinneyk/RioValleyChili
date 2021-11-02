using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    internal static class RouteHelpers
    {
        public static string GetAreaName(this RouteBase route)
        {
            return route.GetDataTokenValue("area");
        }

        public static string GetControllerName(this RouteBase route)
        {
            return route.GetDefaultValue("controller");
        }

        public static string GetActionName(this RouteBase route)
        {
            return route.GetDefaultValue("action");
        }

        private static string GetDataTokenValue(this RouteBase route, string key)
        {
            var castRoute = route as Route;
            object rawValue = null;
            if (castRoute != null && castRoute.DataTokens != null && castRoute.DataTokens.TryGetValue(key, out rawValue))
            {
                return rawValue as string;
            }
            return null;
        }

        private static string GetDefaultValue(this RouteBase route, string key)
        {
            var castRoute = route as Route;
            object rawValue = null;
            if (castRoute != null && castRoute.Defaults != null && castRoute.Defaults.TryGetValue(key, out rawValue))
            {
                return rawValue as string;
            }
            return null;
        }

        public static Route GetRoute(this IEnumerable<RouteBase> routeCollection, string areaName, string controllerName, string actionName)
        {
            var routes = String.IsNullOrWhiteSpace(areaName)
                ? routeCollection
                : FilterRoutesByAreaName(routeCollection, areaName);

            return (from routeBase in routes.Where(r => r != null)
                let thisRoute = routeBase as Route
                let controller = thisRoute.GetControllerName() ?? string.Empty
                let action = thisRoute.GetActionName() ?? string.Empty
                    where controller.Equals(controllerName, StringComparison.OrdinalIgnoreCase)
                      && action.Equals(actionName, StringComparison.OrdinalIgnoreCase)
                select thisRoute).FirstOrDefault();
        }

        public static IEnumerable<RouteBase> FilterRoutesByAreaName(IEnumerable<RouteBase> routes, string areaName)
        {
            return from route in routes 
                let area = route.GetAreaName() ?? String.Empty 
                where String.Equals(areaName, area, StringComparison.OrdinalIgnoreCase) 
                select route;
        }
    }
}