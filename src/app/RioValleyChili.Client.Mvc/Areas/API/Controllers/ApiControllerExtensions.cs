using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public static class ApiControllerExtensions
    {
        public static KeyValuePair<string, Link> BuildSelfLink(this ApiController controller, string id, string rel = "movement-details")
        {
            return new KeyValuePair<string, Link>(rel, new Link
                {
                    HRef = controller.Url.Route("DefaultApi", new { controller = controller.GetType().Name, id })
                });
        }

        public static KeyValuePair<string, Link> BuildReportLink(this ApiController controller, ActionResult actionResult, string rel)
        {
            return new KeyValuePair<string, Link>(rel, new Link
                {
                    HRef = controller.Url.Route("Default", actionResult.GetRouteValueDictionary())
                });
        }

        public static KeyValuePair<string, Link> BuildReportLink(this ApiController controller, object routeValues, string rel)
        {
            return new KeyValuePair<string, Link>(rel, new Link
                {
                    HRef = controller.Url.Route("Default", routeValues)
                });
        }
    }
}