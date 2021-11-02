using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Solutionhead.Services;

namespace RioValleyChili.Client.Mvc.Core
{
    public static class ServiceResultHandler 
    {
        public static void GetHttpResponse(IResult response)
        {
            switch (response.State)
            {
                case ResultState.Success:
                    break;
                case ResultState.Invalid:
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
                case ResultState.Failure:
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));
                case ResultState.NoWorkRequired:
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotModified));
                default:
                    throw new InvalidOperationException(string.Format("The service result '{0}' is not handled by the ServiceResultHandler.", response.State));
            }
        }
    }
}
