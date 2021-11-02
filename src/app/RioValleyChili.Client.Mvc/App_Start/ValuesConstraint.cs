using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace RioValleyChili.Client.Mvc.App_Start
{
    public class ValuesConstraint : IHttpRouteConstraint
    {
        private readonly string[] _validOptions;

        public ValuesConstraint(string options)
        {
            _validOptions = options.Split('|');
        }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            object value;
            if(values.TryGetValue(parameterName, out value) && value != null)
            {
                return _validOptions.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}