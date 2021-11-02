using System.Web.Mvc;
using System.Web.Routing;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore
{
    public interface IActionLinkAdapter
    {
        RouteValueDictionary RouteValues { get; }

        string ActionUrl { get; set; }
    }
}