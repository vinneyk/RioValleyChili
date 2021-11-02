using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore.Helpers
{
    public static class HttpResponseMessageHelper
    {
        public static bool SetHeaderLocation(this HttpResponseMessage response, HttpRequestMessage request, ActionResult result, NameValueCollection queryStringParams = null)
        {
            var url = new System.Web.Http.Routing.UrlHelper(request);

            var routeVals = result.GetRouteValueDictionary();
            var responseRoute = url.Route(null, routeVals);
            if (!string.IsNullOrWhiteSpace(responseRoute))
            {
                var routeUri = new Uri(request.RequestUri, responseRoute);
                var query = HttpUtility.ParseQueryString(routeUri.Query);
                if (queryStringParams != null)
                {
                    query.Add(queryStringParams);
                    responseRoute = string.Format("{0}{1}",
                                                  routeUri.AbsolutePath,
                                                  query.HasKeys()
                                                      ? string.Format("?{0}", query)
                                                      : string.Empty);
                }

                response.Headers.Location = new Uri(request.RequestUri, responseRoute);
                return true;
            }

            return false;
        }
    }
}