using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using FilterAttribute = System.Web.Http.Filters.FilterAttribute;
using IAuthorizationFilter = System.Web.Http.Filters.IAuthorizationFilter;

namespace RioValleyChili.Client.Mvc.Core.Filters
{
    /// <summary>
    /// Enables anti-forgery token validation without the existence of a form (or form element). 
    /// Based on StackOverflow post: 
    /// http://stackoverflow.com/questions/11725988/problems-implementing-validatingantiforgerytoken-attribute-for-web-api-with-mvc/11726560#11726560
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateAntiForgeryTokenFromCookieAttribute : FilterAttribute, IAuthorizationFilter
    {
        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            try
            {
                var headers = actionContext.Request.Headers;
                var cookies = headers.GetCookies().ToList();
                var cookieStates = cookies.Select(c => c[AntiForgeryConfig.CookieName]).ToList();
                if(cookieStates.Count != 1)
                {
                    ElmahExceptionLogger.DefaultLog(new Exception(string.Format("Expected single anti-forgery cookie state, but found {0}.", cookieStates.Count)));
                }

                var tokenValues = headers.GetValues(AntiForgeryConfig.CookieName).ToList();
                var validationResults = cookieStates.SelectMany(c => tokenValues.Select(t => Validate(c, t))).ToList();
                if(!validationResults.Any(r => r))
                {
                    validationResults = tokenValues.Select(t => Validate(null, t)).ToList();
                    if(!validationResults.Any(r => r))
                    {
                        actionContext.Response = new HttpResponseMessage
                            {
                                StatusCode = HttpStatusCode.Forbidden,
                                RequestMessage = actionContext.ControllerContext.Request
                            };
                        return fromResult(actionContext.Response);
                    }
                }
                return continuation();
            }
            catch(Exception ex)
            {
                ElmahExceptionLogger.DefaultLog(ex);
                actionContext.Response = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        RequestMessage = actionContext.ControllerContext.Request
                    };
                return fromResult(actionContext.Response);
            }
        }

        private static bool Validate(CookieState cookie, string token)
        {
            var cookieValue = cookie != null ? cookie.Value : null;
            try
            {
                AntiForgery.Validate(cookieValue, token);
            }
            catch(Exception ex)
            {
                ElmahExceptionLogger.DefaultLog(new Exception(
                    string.Format("Failed validation using cookie[{0}] and token[{1}]", cookieValue, token)
                    , ex));
                return false;
            }

            return true;
        }

        private static Task<HttpResponseMessage> fromResult(HttpResponseMessage result)
        {
            var source = new TaskCompletionSource<HttpResponseMessage>();
            source.SetResult(result);
            return source.Task;
        }
    }
}