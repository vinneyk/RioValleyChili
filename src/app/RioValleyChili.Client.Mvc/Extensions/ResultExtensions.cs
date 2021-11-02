using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Core;
using Solutionhead.Services;
using ApplicationException = Elmah.ApplicationException;

namespace RioValleyChili.Client.Mvc.Extensions
{
    [ExtractIntoSolutionheadLibrary]
    public static class ResultExtensions
    {
        public static bool Success(this IResult result)
        {
            return result != null && result.State == ResultState.Success;
        }

        public static HttpStatusCodeResult ToHttpStatusCodeResult(this IResult result, HttpVerbs verb = HttpVerbs.Get)
        {
            var message = (result.Message ?? "").Replace("\n", "<br />");
            return new HttpStatusCodeResult(result.ToHttpStatusCode(verb), message);
        }

        public static void EnsureSuccess(this IResult result)
        {
            if(!result.Success)
            {
                throw new ApplicationException(result.Message ?? string.Format("The requested action failed with the following state: '{0}'.", result.State));
            }
        }

        /// <summary>
        /// Throws HttpResponseException if the result is not in a successful state.
        /// </summary>
        public static void EnsureSuccessWithHttpResponseException(this IResult result, HttpVerbs verb = HttpVerbs.Get)
        {
            if(!result.Success)
            {
                throw new HttpResponseException(result.ToHttpResponseMessage(verb));
            }
        }

        /// <summary>
        /// Creates a new HttpResponseMessage from the supplied IResult object.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="verb">The HTTP verb description of the API method.</param>
        /// <returns></returns>
        public static HttpResponseMessage ToHttpResponseMessage(this IResult result, HttpVerbs verb = HttpVerbs.Get)
        {
            var statusCode = result.ToHttpStatusCode(verb);
            var message = (result.Message ?? "").Replace("\n", "<br />");
            return new HttpResponseMessage(statusCode)
                {
                    Content = string.IsNullOrEmpty(message)
                        ? new StringContent(string.Format("{{\"status\":\"{0}\"}}", statusCode))
                        : new StringContent(string.Format("{{\"status\":\"{0}\",\"message\":\"{1}\"}}", statusCode, message)),
                    ReasonPhrase = message
                };
        }

        public static HttpResponseMessage ToHttpResponseMessage<TResult>(this IResult<TResult> result, HttpVerbs verb = HttpVerbs.Get)
        {
            var statusCode = result.ToHttpStatusCode(verb);
            return new HttpResponseMessage(statusCode)
                {
                    Content = result.ResultingObject.ToJSONContent(),
                    ReasonPhrase = result.Message
                };
        }

        public static MappedResponseBuilder<TResult> ToMapped<TResult>(this IResult<TResult> result)
        {
            return new MappedResponseBuilder<TResult>(result);
        }

        public class MappedResponseBuilder<TSource>
        {
            private readonly IResult<TSource> _result;

            public MappedResponseBuilder(IResult<TSource> result)
            {
                _result = result;
            }

            public HttpResponseMessage Response<TDestination>(HttpVerbs verb = HttpVerbs.Get)
            {
                var statusCode = _result.ToHttpStatusCode(verb);
                var message = (_result.Message ?? "").Replace("\n", "<br />");
                return new HttpResponseMessage(statusCode)
                    {
                        Content = _result.ResultingObject.Map().To<TDestination>().ToJSONContent(),
                        ReasonPhrase = message
                    };
            }
        }

        public static HttpStatusCode ToHttpStatusCode(this IResult result, HttpVerbs verb = HttpVerbs.Get)
        {
            switch (result.State)
            {
                case ResultState.Success:
                    {
                        switch (verb)
                        {
                            case HttpVerbs.Post: return HttpStatusCode.Created;
                            default: return HttpStatusCode.OK;
                        }
                    }

                case ResultState.Invalid:
                    {
                        switch (verb)
                        {
                            case HttpVerbs.Get: 
                            case HttpVerbs.Delete: 
                                return HttpStatusCode.NotFound;
                                
                            default: return HttpStatusCode.BadRequest;
                        }
                    }

                case ResultState.Failure: return HttpStatusCode.InternalServerError; 
                case ResultState.NoWorkRequired: return HttpStatusCode.NotModified;
                default: throw new NotImplementedException(string.Format("There is no known conversion of the provided ResultState to an HttpStatusCode. ResultState received: '{0}'.", result.State));
            }
        }
    }
}