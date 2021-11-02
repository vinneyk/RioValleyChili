using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using RioValleyChili.Client.Mvc.Core.Security;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public static class NavigationHelper
    {
        public static MvcHtmlString ActionLinkIfAuthorized(this HtmlHelper htmlHelper, NavigationItem navigationItem)
        {
            return IsAuthorized(htmlHelper, navigationItem.RouteValueDictionary)
                ? htmlHelper.ActionLink(navigationItem)
                : MvcHtmlString.Empty;
        }
            
        public static MvcHtmlString BuildNavigation(this HtmlHelper htmlHelper, IEnumerable<NavigationItem> navigationItems, object htmlAttributes = null)
        {
            return BuildNavigation(htmlHelper, navigationItems, false, htmlAttributes);
        }

        public static MvcHtmlString BuildNavigation(this HtmlHelper htmlHelper, IEnumerable<NavigationItem> navigationItems, bool bypassSecurity, object htmlAttributes = null)
        {
            var container = new TagBuilder("ul");
            container.ApplyHtmlAttributes(htmlAttributes);
            
            var innerHtmlBuilder = new StringBuilder();
            foreach (var item in navigationItems.Where(item => bypassSecurity || IsAuthorized(htmlHelper, item.RouteValueDictionary)))
            {
                innerHtmlBuilder.Append(
                    new TagBuilder("li")
                    {
                        InnerHtml = htmlHelper.ActionLink(item).ToHtmlString(),
                    });
            }
            container.InnerHtml = innerHtmlBuilder.ToString();
            return new MvcHtmlString(container.ToString());
        }

        private static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, NavigationItem navigationItem)
        {
            return htmlHelper.ActionLink(
                navigationItem.LinkText,
                navigationItem.RouteValueDictionary["action"] as string,
                navigationItem.RouteValueDictionary["controller"] as string,
                navigationItem.RouteValueDictionary,
                navigationItem.HtmlAttributes.ToHtmlAttributeDictionary());
        }

        private static bool IsAuthorized(this HtmlHelper htmlHelper, RouteValueDictionary routeValues)
        {
            var routeData = BuildRouteData(htmlHelper.RouteCollection, routeValues);
            var context = BuildRequestContext(htmlHelper, routeData);
            return ClaimsAuthorizationHelper.CheckAccess(context);
        }

        private static RouteData BuildRouteData(IEnumerable<RouteBase> routeCollection, RouteValueDictionary routeValues)
        {
            object controllerValue;
            routeValues.TryGetValue("controller", out controllerValue);
            var controllerName = controllerValue as string;

            object actionValue;
            routeValues.TryGetValue("action", out actionValue);
            var actionName = actionValue as String;

            object areaValue;
            routeValues.TryGetValue("area", out areaValue);
            var areaName = areaValue as String ?? "";

            var routeData = new RouteData();
            routeData.Values.Add("action", actionName);
            routeData.Values.Add("controller", controllerName);
            routeData.Values.Add("area", areaName);
            AddNamespaceInfo(routeData, routeCollection, areaName, controllerName, actionName);

            return routeData;
        }

        private static RequestContext BuildRequestContext(this HtmlHelper htmlHelper, RouteData routeData)
        {
            var claimsPrincipal = htmlHelper.ViewContext.HttpContext.User as ClaimsPrincipal;
            var requestContext = new RequestContext(htmlHelper.ViewContext.HttpContext, routeData);
            requestContext.HttpContext.User = claimsPrincipal;

            return requestContext;
        }

        private static void AddNamespaceInfo(RouteData routeData, IEnumerable<RouteBase> routeCollection, string areaName, string controllerName, string actionName)
        {
            var route = routeCollection.GetRoute(areaName, controllerName, actionName);

            if (route != null)
            {
                routeData.DataTokens.Add("Namespaces", route.DataTokens["Namespaces"]);
            }
        }

        public static MvcHtmlString SubNavigation(this HtmlHelper htmlHelper, string title, IEnumerable<NavigationItem> links, object htmlAttributes = null)
        {
            return SubNavigation(htmlHelper, title, links, false, htmlAttributes);
        }

        public static MvcHtmlString SubNavigation(this HtmlHelper htmlHelper, string title, IEnumerable<NavigationItem> links, bool bypassSecurity, object htmlAttributes = null)
        {
            var container = new TagBuilder("nav");
            container.InnerHtml += String.Format("<h3>{0}</h3>", title);
            container.InnerHtml += htmlHelper.BuildNavigation(links, bypassSecurity, new { @class = "link-list" }).ToString();

            var additionalHtmlAttibutes = htmlAttributes.ToHtmlAttributeDictionary();
            container.MergeAttributes(additionalHtmlAttibutes);

            return MvcHtmlString.Create(container.ToString());
        }

        [Obsolete("Use overload accepting IEnumerable<NavigationItems> instead.")]
        public static MvcHtmlString SubNavigation(this HtmlHelper htmlHelper, string title, IEnumerable<MvcHtmlString> links, object htmlAttributes = null)
        {
            var linkList = new TagBuilder("ul");
            linkList.AddCssClass("link-list");
            foreach (var link in links)
            {
                linkList.InnerHtml += String.Format("<li>{0}</li>", link.ToHtmlString());
            }

            var container = new TagBuilder("nav");
            container.InnerHtml += String.Format("<h3>{0}</h3>", title);
            container.InnerHtml += linkList.ToString();
            var additionalHtmlAttibutes = new RouteValueDictionary(htmlAttributes);
            additionalHtmlAttibutes.Where(attr => attr.Key.Contains("_")).ToList()
                .ForEach(attr =>
                {
                    additionalHtmlAttibutes.Add(attr.Key.Replace("_", "-"), attr.Value);
                    additionalHtmlAttibutes.Remove(attr.Key);
                });
            container.MergeAttributes(additionalHtmlAttibutes);

            return MvcHtmlString.Create(container.ToString());
        }
    }
}