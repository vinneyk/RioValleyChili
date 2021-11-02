using System.Web.Mvc;
using System.Web.Routing;

namespace RioValleyChili.Client.Mvc.Core.Security
{
    public class NavigationItem
    {
        #region constructors

        public NavigationItem() { }

        public NavigationItem(string linkText, ActionResult actionResult, object htmlAttributes = null)
            : this(linkText, actionResult.GetRouteValueDictionary(), htmlAttributes) { }

        public NavigationItem(string linkText, RouteValueDictionary routeValues, object htmlAttributes = null)
        {
            LinkText = linkText;
            RouteValueDictionary = routeValues;
            HtmlAttributes = htmlAttributes;
        }

        #endregion

        public string LinkText { get; set; }
        public RouteValueDictionary RouteValueDictionary { get; set; }
        public object HtmlAttributes { get; set; }
    }
}